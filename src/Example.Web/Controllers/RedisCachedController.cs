using System.Collections.Generic;
using Example.Services.Interfaces;
using Example.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Example.Web.Controllers
{
    public class RedisCachedController : Controller
    {
        private readonly IHorseService _horseService;
        private readonly IMapper<Dto.Horse, Models.HorseSummary> _horseSummaryMapper;

        public RedisCachedController(IHorseService horseService,
            IMapper<Dto.Horse, Models.HorseSummary> horseSummaryMapper)
        {
            _horseService = horseService;
            _horseSummaryMapper = horseSummaryMapper;
        }

        public ActionResult Index()
        {
            const string cachekey = "REDIS_CACHED";
            byte[] cached;
            string value;

            ViewBag.Message = "Redis Cached";

            if (HttpContext.Session.TryGetValue(cachekey, out cached))
            {
                // Get the value from cache
                value = Encoding.UTF8.GetString(cached);

                ViewBag.Message = "Redis Cached - From Cache";
            }
            else
            {
                // Get the values to be cached
                var horses = _horseService.GetAll();

                var mappedHorses = horses.Select(_horseSummaryMapper.Map).ToList();

                value = JsonConvert.SerializeObject(mappedHorses);

                var encodedHorses = Encoding.UTF8.GetBytes(value);

                HttpContext.Session.Set(cachekey, encodedHorses);
            }

            var model = JsonConvert.DeserializeObject<List<Models.HorseSummary>>(value);

            return View("Horses", model);
        }
    }
}