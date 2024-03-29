﻿using Servirtium.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Servirtium.Climate.Demo
{ 
    internal class ClimateApi
    {
        public static readonly Uri DOCKER_SITE = new Uri("http://worldbank-api-for-servirtium.local.gd:4567");
        public static readonly Uri GITHUB_STATIC_SITE = new Uri("https://servirtium.github.io/worldbank-climate-recordings");

        private readonly Uri _site;
        private readonly HttpClient _client;
        public ClimateApi(Uri site) : this (new HttpClient { Timeout = TimeSpan.FromSeconds(5) }, site) {}

        public ClimateApi(HttpClient client) : this(client, DOCKER_SITE) { }

        public ClimateApi(HttpClient client, Uri site)
        {
            _client = client;
            _site = site;
        }

        public async Task<double> GetAveAnnualRainfall(int fromCCYY, int toCCYY, params string[] countryIsos)
        {
            IEnumerable<Task<double>> avgTempPerCountry = countryIsos.Select(async countryIso =>
            {
                var requestUri = new Uri(_site, $"/climateweb/rest/v1/country/annualavg/pr/{fromCCYY}/{toCCYY}/{countryIso}.xml");
                var response = await _client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    var rawXml = await response.Content.ReadAsStringAsync();
                    if (rawXml.Contains("Invalid country code. Three letters are required"))
                    {
                        throw new Exception($"{countryIso} not recognized by climateweb");
                    }
                    var doc = XDocument.Parse(rawXml);
                    var result = doc.Descendants(XNamespace.None + "annualData")
                        .Descendants(XNamespace.None + "double")
                        .Select(xe => Double.Parse(xe.Value));
                    if (!result.Any())
                    {
                        throw new Exception($"date range {fromCCYY}-{toCCYY} not supported");
                    }
                    return result.Average();
                }
                else throw new HttpRequestException($"GET Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");
            });
            //'Average of averages' logic replicates https://github.com/servirtium/demo-java-climate-data-tck/blob/master/src/main/java/com/paulhammant/climatedata/ClimateApi.java
            List<double> averages = new List<double>();
            foreach(var calculateAverage in avgTempPerCountry)
            {
                averages.Add(await calculateAverage);
            }
            return averages.Average();

        }

        public async Task<double> GetPlanetaryRainfall(int fromCCYY, int toCCYY, string planet)
        {
            var requestUri = new Uri(_site, $"/climateweb/rest/v1/planet/annualavg/pr/{fromCCYY}/{toCCYY}/{planet}.xml");
            var response = await _client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var rawXml = await response.Content.ReadAsStringAsync();
                var doc = XDocument.Parse(rawXml);
                var result = doc.Descendants(XNamespace.None + "annualData")
                    .Descendants(XNamespace.None + "double")
                    .Select(xe => Double.Parse(xe.Value));
                if (!result.Any())
                {
                    throw new Exception($"date range {fromCCYY}-{toCCYY} not supported");
                }
                return result.Average();
            }
            else throw new HttpRequestException($"GET Request to {requestUri} failed, status {response.StatusCode}, Content: {Environment.NewLine}{await response.Content.ReadAsStringAsync()}");

        }
        public static Uri GetRealServiceUrl()
        {
            return File.Exists(".useDockerHostedRealService") 
                ? ClimateApi.DOCKER_SITE
                : ClimateApi.GITHUB_STATIC_SITE;
        }

    }
}