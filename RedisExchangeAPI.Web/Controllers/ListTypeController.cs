using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class ListTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;
        private string _listKey = "names";

        public ListTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(1);
        }


        public IActionResult Index()
        {
            List<string> nameList = new List<string>();
            if (db.KeyExists(_listKey))
            {
                db.ListRange(_listKey).ToList().ForEach(x =>
                {
                    nameList.Add(x.ToString());

                });
            }
            return View(nameList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            db.ListLeftPush(_listKey, name);
            return RedirectToAction("Index");
        }


        public IActionResult DeleteItem(string name)
        {
            db.ListRemoveAsync(_listKey, name).Wait();
            return RedirectToAction("Index");
        }

        public IActionResult DeleteFirstItem()
        {
            db.ListLeftPop(_listKey);

            return RedirectToAction("Index");
        }
    }
}