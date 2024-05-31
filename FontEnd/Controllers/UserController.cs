using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QLMP.Common.Req;
using System.Text;

namespace FrontEnd.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7279/api/");
         
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var token = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            List<KhachHangVM> khachHangVMs = new List<KhachHangVM>();
            HttpResponseMessage response = await _httpClient.GetAsync("KhachHang/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                JObject jsonResponse = JObject.Parse(responseData);

                if (jsonResponse["data"] != null && jsonResponse["data"].Type == JTokenType.Array)
                {
                    khachHangVMs = jsonResponse["data"].ToObject<List<KhachHangVM>>();
                }
            }

            return View(khachHangVMs);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int uid)
        {
            var token = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.DeleteAsync($"User/DeleteUserById?id={uid}");
            if (response.IsSuccessStatusCode)
            {

                TempData["SuccessMessage"] = "Delete successfully.";
                return Redirect("/User");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return Redirect("/User");
            }
        }
        public async Task<IActionResult> Update(int id)
        {
            var token = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.GetAsync($"KhachHang/GetById?id={id}");
            if (response.IsSuccessStatusCode)
            {
                var productData = await response.Content.ReadAsStringAsync();
                var cate = JsonConvert.DeserializeObject<KhachHangReq>(productData);
                return View(cate);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, KhachHangReq model)
        {
            var token = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"KhachHang/Update?Id={id}", content);

            if (response.IsSuccessStatusCode)
            {

                TempData["SuccessMessage"] = "Update successfully.";
                return Redirect("/User");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
                return Redirect("/User");
            }
        }
    }
}
