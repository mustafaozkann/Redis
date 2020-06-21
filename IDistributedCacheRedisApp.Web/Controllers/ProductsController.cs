using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private IDistributedCache _distributedCache;
        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;

        }
        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
            };

            _distributedCache.SetString("name", "mustafa", cacheEntryOptions);
            await _distributedCache.SetStringAsync("surname", "ozkan");

            var product = new Product()
            {
                Id = 1,
                Name = "Kalem",
                Price = 50
            };
            string jsonProduct = JsonConvert.SerializeObject(product);

            Byte[] byteProduct = Encoding.UTF8.GetBytes(jsonProduct);

            _distributedCache.Set("productByte:1", byteProduct);

            await _distributedCache.SetStringAsync("product:1", jsonProduct, cacheEntryOptions);
            return View();
        }

        public IActionResult Show()
        {
            string jsonProduct = _distributedCache.GetString("product:1");
            var product = JsonConvert.DeserializeObject<Product>(jsonProduct);
            ViewBag.Product = product;

            Byte[] byteProduct = _distributedCache.Get("productByte:1");
            string byteJsonProduct = Encoding.UTF8.GetString(byteProduct);
            var productByte = JsonConvert.DeserializeObject<Product>(jsonProduct);

            ViewBag.ProductByte = productByte;
            string name = _distributedCache.GetString("name");
            ViewBag.Name = name;

            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("name");
            return View();
        }

        public IActionResult ImageUrl()
        {
            byte[] imageByte = _distributedCache.Get("image");

            return File(imageByte, "image/jpg");
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/download.jpg");

            Byte[] imageByte = System.IO.File.ReadAllBytes(path);

            _distributedCache.Set("image", imageByte);
            return View();
        }
    }
}