using System;
using System.Collections.Generic;
using Example.Models;
using Example.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Example.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;
        protected DbSet<T> DbSet;
        private readonly IMemoryCache _memoryCache;

        public Repository(ExampleContext context, IMemoryCache memoryCache)
        {
            Context = context;
            _memoryCache = memoryCache;
            DbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);

            Save();
        }

        public T Get<TKey>(TKey id)
        {
            return DbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public void Update(T entity)
        {
            Save();
        }

        private void Save()
        {
            Context.SaveChanges();
        }

        public List<T> GetAllCached()
        {
            const string cachekey = "REPOSITORY_CACHED";
            List<T> items;

            if (!_memoryCache.TryGetValue(cachekey, out items))
            {
                // Get the values to be cached
                items = GetAll().ToList();

                // Decide how to cache it
                var opts = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };

                // Store it in cache
                _memoryCache.Set(cachekey, items, opts);
            }

            return items;
        }
    }
}
