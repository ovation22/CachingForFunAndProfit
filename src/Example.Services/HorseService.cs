using System;
using Example.Repositories.Interfaces;
using Example.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Example.Services
{
    public class HorseService : IHorseService
    {
        private readonly IRepository<Models.Horse> _horseRepository;
        private readonly IMemoryCache _memoryCache;

        public HorseService(IRepository<Models.Horse> horseRepository, IMemoryCache memoryCache)
        {
            _horseRepository = horseRepository;
            _memoryCache = memoryCache;
        }

        public IEnumerable<Dto.Horse> GetAll()
        {
            var horses = _horseRepository.GetAll();

            return horses.Select(Map);
        }

        public IEnumerable<Dto.Horse> GetAllCached()
        {
            const string cachekey = "SERVICE_CACHED";
            List<Dto.Horse> horses;

            if (!_memoryCache.TryGetValue(cachekey, out horses))
            {
                // Get the values to be cached
                horses = GetAll().ToList();

                // Decide how to cache it
                var opts = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };

                // Store it in cache
                _memoryCache.Set(cachekey, horses, opts);
            }

            return horses;
        }

        public Dto.Horse Get(int id)
        {
            var horse = _horseRepository.Get(id);

            return horse == null ? null : Map(horse);
        }

        public Dto.Horse GetCached(int id)
        {
            return GetAllCached().SingleOrDefault(x => x.Id == id);
        }

        public IEnumerable<Dto.Horse> GetAllRepositoryCached()
        {
            var horses = _horseRepository.GetAllCached();

            return horses.Select(Map);
        }

        public Dto.Horse GetRepositoryCached(int id)
        {
            return GetAllRepositoryCached().SingleOrDefault(x => x.Id == id);
        }

        private static Dto.Horse Map(Models.Horse horse)
        {
            return new Dto.Horse
            {
                Id = horse.Id,
                Name = horse.Name,
                Starts = horse.RaceStarts,
                Win = horse.RaceWins,
                Place = horse.RacePlace,
                Show = horse.RaceShow,
                Earnings = horse.Earnings
            };
        }
    }
}
