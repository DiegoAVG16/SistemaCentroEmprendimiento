using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;
using CentroEmpEntidades;


namespace CentroEmpWeb.Controllers
{
    public class ChatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey = "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";

        public ChatController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string prompt)
        {
            string respuestaFija = ObtenerRespuestaFija(prompt);
            if (respuestaFija != null)
            {
                ViewBag.Response = respuestaFija;
                return View();
            }

            // Si no hay respuesta fija, usar OpenRouter
            var client = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                model = "mistralai/mistral-7b-instruct:free",
                messages = new[]
                {
            new { role = "user", content = prompt }
        }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Headers.Add("HTTP-Referer", "https://localhost");
            request.Headers.Add("X-Title", "CentroEmpWeb");
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                dynamic error = JsonConvert.DeserializeObject(responseContent);
                ViewBag.Response = "Error: " + (string)error?.error?.message ?? "Error inesperado";
                return View();
            }

            dynamic result = JsonConvert.DeserializeObject(responseContent);
            ViewBag.Response = (string)result.choices[0].message.content;

            return View();
        }

        private string ObtenerRespuestaFija(string prompt)
        {
            prompt = prompt.ToLower();

            if (prompt.Contains("1"))
                return "Puedes crear una cita desde el menú 'Click Opción > Mis Citas' > 'Nueva cita' > Elegir " +
                    "Especialidad > Seleccionar asesor > Seleccionar el horario disponible.";

            if (prompt.Contains("2"))
                return "Puedes  ver el historial desde el menú 'Click Opción > Historial citas";

            

            return null; // Si no hay coincidencia, usar la API
        }

    }
}
