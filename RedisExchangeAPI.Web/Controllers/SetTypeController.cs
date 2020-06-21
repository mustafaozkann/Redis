using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{

    public class SetTypeController : Controller
    {

        private readonly RedisService _redisService;
        private readonly IDatabase db;

        private string _listKey = "hashNames";


        public SetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(2);
        }


        public IActionResult Index()
        {
            HashSet<string> nameList = new HashSet<string>();

            if (db.KeyExists(_listKey))
            {
                db.SetMembers(_listKey).ToList().ForEach(x =>
                {
                    nameList.Add(x);
                });
            }
            return View(nameList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            db.SetAdd(_listKey, name);
            if (!db.KeyExists(_listKey))
            {
                db.KeyExpire(_listKey, DateTime.Now.AddMinutes(5));
            }

            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteItem(string name)
        {
            if (db.KeyExists(_listKey))
            {
                await db.SetRemoveAsync(_listKey, name);
            }
            return RedirectToAction("Index");
        }
    }
}