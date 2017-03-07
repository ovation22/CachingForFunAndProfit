using System;
using System.Collections.Generic;
using Example.Services.Interfaces;
using Example.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Example.Web.Controllers
{
    public class MemoryCachedController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHorseService _horseService;
        private readonly IMapper<Dto.Horse, Models.HorseSummary> _horseSummaryMapper;

        public MemoryCachedController(IMemoryCache memoryCache,
            IHorseService horseService,
            IMapper<Dto.Horse, Models.HorseSummary> horseSummaryMapper)
        {
            _memoryCache = memoryCache;
            _horseService = horseService;
            _horseSummaryMapper = horseSummaryMapper;
        }

        public ActionResult Index()
        {
            const string cachekey = "CONTROLLER_CACHED";
            List<Models.HorseSummary> cached;

            if (!_memoryCache.TryGetValue(cachekey, out cached))
            {
                // Get the values to be cached
                var horses = _horseService.GetAll();

                cached = horses.Select(_horseSummaryMapper.Map).ToList();

                // Decide how to cache it
                var opts = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };

                // Store it in cache
                _memoryCache.Set(cachekey, cached, opts);
            }

            ViewBag.Message = "Memory Cached";

            return View("Horses", cached);
        }
    }
}
