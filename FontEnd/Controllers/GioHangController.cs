using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QLMP.Common.Req;
using System.Text;

namespace FrontEnd.Controllers
{
    public class GioHangController : Controller
    {
        private readonly HttpClient _httpClient;

        public GioHangController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7279/api/");
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return Redirect("/Login");
            }
            if (role == "admin")
            {
                return Redirect("/");
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"KhachHang/GetByUserID?UserId={int.Parse(userId)}");
            KhachHangVM kh = null;
            if (response.IsSuccessStatusCode)
            {
                
                var responseData = await response.Content.ReadAsStringAsync();

                // Parse the JSON object
                JObject jsonResponse = JObject.Parse(responseData);

                // Check if the "data" property exists and is not null
                if (jsonResponse["data"] != null)
                {
                    // Deserialize the data into a CartReq object
                    kh = jsonResponse["data"].ToObject<KhachHangVM>();
                }
            }

            var addToCartReq = new
            {
                Makh = kh.MaKh,
                ProductId = id,
                Quantity = 1
            };


            var json = JsonConvert.SerializeObject(addToCartReq);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response1 = await _httpClient.PostAsync("Cart/add-product", content);

            if (response1.IsSuccessStatusCode)
            {
                return Redirect("/GioHang");
            }
            else
            {
                // Log error or return a specific error view
                var errorContent = await response1.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {errorContent}");
                return Redirect("/");
            }
        }
       
        public async Task<IActionResult> Index()
        {

            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                return Redirect("/Login");
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"KhachHang/GetByUserID?UserId={int.Parse(userId)}");
            KhachHangVM kh = null;
            if (response.IsSuccessStatusCode)
            {

                var responseData = await response.Content.ReadAsStringAsync();

                // Parse the JSON object
                JObject jsonResponse = JObject.Parse(responseData);

                // Check if the "data" property exists and is not null
                if (jsonResponse["data"] != null)
                {
                    // Deserialize the data into a CartReq object
                    kh = jsonResponse["data"].ToObject<KhachHangVM>();
                }
            }
            CartReq cartReq = null;
            HttpResponseMessage response1 = await _httpClient.GetAsync($"Cart/get-cart-by-id?userId={kh.MaKh}");
            if (response1.IsSuccessStatusCode)
            {
                var responseData = await response1.Content.ReadAsStringAsync();

                // Parse the JSON object
                JObject jsonResponse = JObject.Parse(responseData);

                // Check if the "data" property exists and is not null
                if (jsonResponse["data"] != null)
                {
                    // Deserialize the data into a CartReq object
                    cartReq = jsonResponse["data"].ToObject<CartReq>();
                }
            }

            if (cartReq == null)
            {
                // Handle the case where the cart is not found or the response is invalid
                return RedirectToAction("Index", "Home");
            }

            return View(cartReq);
        }
        public async Task<IActionResult> PlaceOrder()
        {

            var userId = HttpContext.Session.GetString("UserId");
            HttpResponseMessage response = await _httpClient.GetAsync($"KhachHang/GetByUserID?UserId={userId}");
            KhachHangVM kh = null;
            if (response.IsSuccessStatusCode)
            {

                var responseData = await response.Content.ReadAsStringAsync();

                // Parse the JSON object
                JObject jsonResponse = JObject.Parse(responseData);

                // Check if the "data" property exists and is not null
                if (jsonResponse["data"] != null)
                {
                    // Deserialize the data into a CartReq object
                    kh = jsonResponse["data"].ToObject<KhachHangVM>();
                }
            }

            if (string.IsNullOrEmpty(kh.MaKh.ToString()))
            {
                return Redirect("/Login");
            }

            // Create the JSON payload
            var json = new { UserId = kh.MaKh };
            var content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");

            HttpResponseMessage response2 = await _httpClient.PostAsync("Cart/place-order", content);

            if (response2.IsSuccessStatusCode)
            {
                return Redirect("/CheckOut");
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"Cart/remove-product?cartItemId={id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product deleted successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";
                return RedirectToAction("Index");
            }
        }
    }
    
}
