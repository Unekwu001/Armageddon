using Armageddon.Server.Common.Dtos;
using Armageddon.Server.Common.Utils;
using Armageddon.Server.Core.Repos.UserRepository;
using Armageddon.Server.Data.Models.UserModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Text.Json;

namespace Armageddon.Server.Core.Hubs
{

    public class SellerHub : Hub
    {
        private readonly IDistributedCache _cache;
        private readonly IUserRepo _userRepo;

        public SellerHub(IDistributedCache cache, IUserRepo userRepo)
        {
            _cache = cache;
            _userRepo = userRepo;
        }

        public async Task FindNearbySellers(double buyerLat, double buyerLng)
        {
            var activeSellers = await GetActiveSellersFromCacheAsync();

            if (activeSellers.Count == 0)
            {
                activeSellers = await LoadSellersIntoCacheAsync();
            }

            var nearbySellers = activeSellers
                .Select(s => new SellerLocationDto
                {
                    Id = s.Id,
                    UserName = s.UserName,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    IsOnline = true,
                    DistanceKm = CalculateDistance(buyerLat, buyerLng, s.Latitude, s.Longitude),
                    Rating = s.Rating
                })
                .OrderByDescending(s => s.IsOnline)
                .ThenBy(s => s.DistanceKm)
                .Take(25)
                .ToList();

            await Clients.Caller.SendAsync("ReceiveNearbySellers", nearbySellers);
        }



        private async Task<List<LiveSellerDto>> GetActiveSellersFromCacheAsync()
        {
            var cached = await _cache.GetStringAsync("active_sellers");
            return string.IsNullOrEmpty(cached)
                ? new List<LiveSellerDto>()
                : JsonSerializer.Deserialize<List<LiveSellerDto>>(cached) ?? new List<LiveSellerDto>();
        }


        private async Task<List<LiveSellerDto>> LoadSellersIntoCacheAsync()
        {
            var sellersFromDb = await _userRepo.GetAllSellersWithLocationAsync();

            var liveSellers = sellersFromDb.Select(s => new LiveSellerDto
            {
                Id = s.Id,
                UserName = s.UserName ?? "Seller",
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Rating = s.Rating
            }).ToList();

            var json = JsonSerializer.Serialize(liveSellers);
            await _cache.SetStringAsync("active_sellers", json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return liveSellers;
        }





        public async Task UpdateMyLocation(double lat, double lng)
        {
            var userId = Context.GetCurrentUserId();

            if (userId == Guid.Empty) return;

            var activeSellers = await GetActiveSellersFromCacheAsync();

            var seller = activeSellers.FirstOrDefault(s => s.Id == userId);
            if (seller != null)
            {
                seller.Latitude = lat;
                seller.Longitude = lng;
            }
            else
            {
                activeSellers.Add(new LiveSellerDto
                {
                    Id = userId,
                    Latitude = lat,
                    Longitude = lng
                });
            }

            var json = JsonSerializer.Serialize(activeSellers);
            await _cache.SetStringAsync("active_sellers", json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetCurrentUserId();

            if (userId != Guid.Empty)
            {
                await _cache.SetStringAsync($"online_{userId}", "true",
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                    });
            }

            await base.OnConnectedAsync();
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth radius in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return Math.Round(R * c, 2);
        }

        private static double ToRadians(double deg) => deg * (Math.PI / 180);


    }



}
