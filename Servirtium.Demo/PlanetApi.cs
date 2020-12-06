﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace Servirtium.Demo
{
    class PlanetApi
    {

        internal static readonly Uri DEFAULT_SITE = new Uri("http://localhost:1001");

        private readonly Uri _site;
        private readonly HttpClient _client;
        public PlanetApi() : this(DEFAULT_SITE) { }
        public PlanetApi(Uri site) : this(new HttpClient { Timeout = TimeSpan.FromSeconds(30) }, site) { }

        public PlanetApi(HttpClient client) : this(client, DEFAULT_SITE) { }

        public PlanetApi(HttpClient client, Uri site)
        {
            _client = client;
            _site = site;
        }


        public async Task<IEnumerable<string>> GetPlanets(string star)
        {
            var requestUri = new Uri(_site, $"/{star}");
            var response = await _client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var bodyStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<string[]>(bodyStream);
                
            }
            else throw new HttpRequestException($"GET Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }

        public async Task<IDictionary<string, string>> GetPlanet(string star, string planet)
        {
            var requestUri = new Uri(_site, $"/{star}/{planet}");
            var response = await _client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var bodyStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(bodyStream);

            }
            else throw new HttpRequestException($"GET Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }

        public async Task RegisterNewPlanet(string star, string planet, Dictionary<string, string> details)
        {
            var requestUri = new Uri(_site, $"/{star}/{planet}");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(details), Encoding.UTF8, "application/json");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            requestMessage.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            var response = await _client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();

            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"POST Request to {requestUri} failed, status {response.StatusCode}, Content: {responseBody}");
            }
        }

        public async Task UpdatePlanet(string star, string planet, Dictionary<string, string> details)
        {
            var requestUri = new Uri(_site, $"/{star}/{planet}");
            var response = await _client.PutAsync(requestUri, new StringContent(JsonSerializer.Serialize(details), Encoding.UTF8, "application/json") );
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();

            }
            else throw new HttpRequestException($"PUT Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }

        public async Task DestroyPlanet(string star, string planet)
        {
            var requestUri = new Uri(_site, $"/{star}/{planet}");
            var response = await _client.DeleteAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();

            }
            else throw new HttpRequestException($"DELETE Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }

        public async Task<Bitmap> GetPhoto(string star, string planet, string photoFile)
        {
            var requestUri = new Uri(_site, $"/{star}/{planet}/photos/{photoFile}");
            var response = await _client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return new Bitmap(await response.Content.ReadAsStreamAsync());

            }
            else throw new HttpRequestException($"GET Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }

        public async Task<string> SendPhoto(string star, string planet, string photoFile, Bitmap photo)
        {
            var requestUri = new Uri(_site, $"/{star}/{planet}/photos/{photoFile}");
            using (var ms = new MemoryStream())
            {
                photo.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                var content = new StreamContent(ms);

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                var response = await _client.PostAsync(requestUri, content);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();

                }            
                else throw new HttpRequestException($"POST Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");

            }
        }

        public async Task OptionsRequest()
        {
            var requestUri = new Uri(_site, $"/boogy/wonderland");
            var request = new HttpRequestMessage(HttpMethod.Options, requestUri);
            request.Headers.Add("Access-Control-Request-Method", "JITTERBUG");
            request.Headers.Add("Access-Control-Request-Headers", "JITTER, BUG, BOOGY");
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();

            }
            else throw new HttpRequestException($"OPTIONS Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
        }
    }
}
