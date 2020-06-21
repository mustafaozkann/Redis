using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class HashTypeController : BaseController
    {
        private string hashKey = "sozluk";

        public HashTypeController(RedisService redisService) : base(redisService)
        {
        }

        public IActionResult Index()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            if (db.KeyExists(hashKey))
            {
                if (db.HashExists(hashKey, "pen"))
                {
                    db.HashGet(hashKey, "pen");
                }
                db.HashGetAll(hashKey).ToList().ForEach(x =>
                {
                    list.Add(x.Name, x.Value);
                });
            }
            return View(list);
        }

        [HttpPost]
        public IActionResult Add(string name, string value)
        {
            db.HashSet(hashKey, name, value);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteItem(string name)
        {
            if (db.KeyExists(hashKey))
            {
                db.HashDelete(hashKey, name);
            }
            return RedirectToAction("Index");
        }
    }
}