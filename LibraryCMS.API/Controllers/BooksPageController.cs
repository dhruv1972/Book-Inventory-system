using LibraryCMS.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace LibraryCMS.API.Controllers
{
    public class BooksPageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BooksPageController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7280/api/"); // Adjust if your port differs
            return client;
        }

        public async Task<IActionResult> Index()
        {
            var client = CreateClient();
            var response = await client.GetAsync("books");

            if (!response.IsSuccessStatusCode) return View(new List<BookDTO>());

            var json = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<BookDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(books);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] string title)
        {
            Console.WriteLine($">>>> Create hit, title = '{title}'");

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Title empty – nothing sent to API");
                return RedirectToAction(nameof(Index));
            }

            var client = CreateClient();
            var content = new StringContent(
                JsonSerializer.Serialize(new { title }),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("books", content);
            Console.WriteLine("POST /api/books → " + response.StatusCode);

            return RedirectToAction(nameof(Index));
        }





        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = CreateClient();
            await client.DeleteAsync($"books/{id}");
            return RedirectToAction("Index");
        }
    }
}
