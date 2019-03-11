using BeerBook.Models.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BeerBook.Web.Clients
{
    public class BasketClient : IBasketClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public BasketClient(HttpClient httpClient, ILogger<BasketClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BasketModel> GetBasket(string user)
        {
            var url = $"api/Basket/{user}";
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new BasketModel()
                {
                    User = user,
                    Beers = Array.Empty<int>()
                };
            }

            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsAsync<BasketModel>();
            return data;
        }

        public async Task AddBeerToBasket(string user, int id)
        {
            var url = $"api/Basket/{user}/beers/{id}";
            var response = await _httpClient.PostAsync(url, new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();
        }
    }
}
