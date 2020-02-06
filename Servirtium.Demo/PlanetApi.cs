﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;

namespace Servirtium.Demo
{
    class PlanetApi
    {

        internal static readonly Uri DEFAULT_SITE = new Uri("http://localhost:1001");

        private readonly Uri _site;

        public PlanetApi() : this(DEFAULT_SITE) { }

        public PlanetApi(Uri site)
        {
            _site = site;
        }

        public async Task<IEnumerable<string>> GetPlanets(string star)
        {

            var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var requestUri = new Uri(_site, $"/{star}");
            var response = await httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var bodyStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<string[]>(bodyStream);
                
            }
            else throw new HttpRequestException($"GET Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }

        public async Task RegisterNewPlanet(string star, string planet, Dictionary<string, string> details)
        {

            var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var requestUri = new Uri(_site, $"/{star}/{planet}");
            var response = await httpClient.PostAsync(requestUri, new StringContent(JsonSerializer.Serialize(details), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();

            }
            else throw new HttpRequestException($"POST Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }

        public async Task UpdatePlanet(string star, string planet, Dictionary<string, string> details)
        {

            var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var requestUri = new Uri(_site, $"/{star}/{planet}");
            var response = await httpClient.PutAsync(requestUri, new StringContent(JsonSerializer.Serialize(details), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();

            }
            else throw new HttpRequestException($"POST Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }

        public async Task DestroyPlanet(string star, string planet)
        {

            var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var requestUri = new Uri(_site, $"/{star}/{planet}");
            var response = await httpClient.DeleteAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();

            }
            else throw new HttpRequestException($"POST Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }
    }
}
