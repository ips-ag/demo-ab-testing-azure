using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Infrastructure.Data.Repositories
{
    public class RedisBasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _distributedCache;

        public RedisBasketRepository(IDistributedCache __distributedCache)
        {
            _distributedCache = __distributedCache;
        }

        public async Task<Basket> GetBasketAsync(string basketId)
        {
            var objectFromCache = await _distributedCache.GetAsync(GetRedisKey(basketId));
            return objectFromCache.IsNullOrEmpty()
                ? null
                : JsonSerializer.Deserialize<Basket>(objectFromCache);
        }

        public async Task<Basket> UpdateBasketAsync(Basket basket)
        {
            await _distributedCache.SetStringAsync(GetRedisKey(basket.Id), JsonSerializer.Serialize(basket));
            return basket;
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            await _distributedCache.RemoveAsync(GetRedisKey(basketId));
            return true;
        }

        private static string GetRedisKey(string basketId) => $"Basket:{basketId}";
    }
}