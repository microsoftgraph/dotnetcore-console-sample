﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;

namespace ConsoleGraphTest
{
    class Program
    {
        private static GraphServiceClient _graphServiceClient;

        static void Main(string[] args)
        {
            // Load appsettings.json
            var config = LoadAppSettings();
            if (null == config)
            {
                Console.WriteLine("Missing or invalid appsettings.json file. Please see README.md for configuration instructions.");
                return;
            }

            //Query using Graph SDK (preferred when possible)
            GraphServiceClient graphClient = GetAuthenticatedGraphClient(config);

            SearchHelperCall(config);
        }

        // Add a private method to do any necessary setup and make calls to your helper
        private static void SearchHelperCall(IConfigurationRoot config)
        {
            string keyword = config["queryKeyword"];
            var searchHelper = new SearchHelper(_graphServiceClient);
            var messageResult = searchHelper.SearchEntityByKeyword(keyword, EntityType.Message).Result;
            //var eventResult = searchHelper.SearchEntityByKeyword(keyword, EntityType.Event).Result;
            //var siteResult = searchHelper.SearchEntityByKeyword(keyword, EntityType.Site).Result;
            //var driveItemResult = searchHelper.SearchEntityByKeyword(keyword, EntityType.DriveItem).Result;

            var hitsContainerEnumerator = messageResult[0].HitsContainers.GetEnumerator();
            hitsContainerEnumerator.MoveNext();
            var hitsEnumerator = hitsContainerEnumerator.Current.Hits.GetEnumerator();
            hitsEnumerator.MoveNext();

            // cast the result resource to the Microsoft Graph type for easier access to properties
            Message message = (Message)hitsEnumerator.Current.Resource;

            Console.WriteLine(message.Subject);
        }

        private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            _graphServiceClient = new GraphServiceClient(authenticationProvider);
            return _graphServiceClient;
        }

        private static IAuthenticationProvider CreateAuthorizationProvider(IConfigurationRoot config)
        {
            var clientId = config["applicationId"];
            var redirectUri = config["redirectUri"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}";

            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var pca = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .WithRedirectUri(redirectUri)
                                                    .Build();
            return new DeviceCodeFlowAuthorizationProvider(pca, scopes);
        }

        private static IConfigurationRoot LoadAppSettings()
        {
            try
            {
                var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

                // Validate required settings
                if (string.IsNullOrEmpty(config["applicationId"]) ||
                    string.IsNullOrEmpty(config["applicationSecret"]) ||
                    string.IsNullOrEmpty(config["redirectUri"]) ||
                    string.IsNullOrEmpty(config["tenantId"]) ||
                    string.IsNullOrEmpty(config["domain"]) ||
                    string.IsNullOrEmpty(config["queryKeyword"]))
                {
                    return null;
                }

                return config;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }
    }
}
