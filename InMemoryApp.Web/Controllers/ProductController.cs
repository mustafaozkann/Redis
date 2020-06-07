using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            // first way
            //if (String.IsNullOrEmpty(_memoryCache.Get<string>("time")))
            //{
            //    _memoryCache.Set<string>("time", DateTime.Now.ToString());

            //}


            // second way
            if (!_memoryCache.TryGetValue("time", out string timeCache))
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
                //options.SlidingExpiration = TimeSpan.FromSeconds(15);
                options.Priority = CacheItemPriority.High;

                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _memoryCache.Set("callback", $"{key}->{value} => sebep : {reason}");
                });

                _memoryCache.Set<string>("time", DateTime.Now.ToString(), options);

                var product = new Product()
                {
                    Id = 1,
                    Name = "Product 1",
                    Price = 150
                };

                _memoryCache.Set<Product>("product:1", product);

            }


            return View();
        }

        public IActionResult Show()
        {
            //_memoryCache.Remove("time");
            //_memoryCache.GetOrCreate<string>("time", entry =>
            //{
            //    entry.Priority = CacheItemPriority.High;
            //    return DateTime.Now.ToString();
            //});

            _memoryCache.TryGetValue("time", out string timeCache);
            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.Time = timeCache;
            ViewBag.Callback = callback;
            ViewBag.Product = _memoryCache.Get<Product>("product:1");
            return View();
        }


    }
}