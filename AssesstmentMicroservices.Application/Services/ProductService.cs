using AssesstmentMicroservices.Application.Interfaces;
using AssesstmentMicroservices.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssesstmentMicroservices.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "product_list";

        public ProductService(IUnitOfWork uow, IMemoryCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            List<Product> products = new List<Product>();
            if (_cache.TryGetValue(CacheKey, out products))
            {
                return products;
            }

            products = (List<Product>?)await _uow.Products.GetAllAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(CacheKey, products, cacheOptions);

            return products;
        }
    }
}
