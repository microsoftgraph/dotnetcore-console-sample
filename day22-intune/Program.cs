using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ConsoleGraphTest
{
    class Program
    {
        private static GraphServiceClient _graphServiceClient;
        private static HttpClient _httpClient;

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

            IntuneHelperCall(config).GetAwaiter().GetResult();
        }

        private static async Task IntuneHelperCall(IConfigurationRoot config)
        {
            const string userPrincipalName = "<user>";

            var graphClient = GetAuthenticatedGraphClient(config);
            var intuneHelper = new IntuneHelper(graphClient);

            await ListManagedDevices(intuneHelper, userPrincipalName);

            WebApp app = await PublishWebApp(
                intuneHelper,
                "http://aka.ms/30DaysMsGraph",
                "30 Days of MS Graph",
                "Microsoft Corporation");

            await AssignAppToAllUsers(intuneHelper, app);

            DeviceConfiguration deviceConfiguration = await CreateWindowsDeviceConfiguration(
                intuneHelper,
                "Windows 10 Developer Configuration",
                "http://aka.ms/30DaysMsGraph",
                true);

            await AssignDeviceConfigurationToAllDevices(intuneHelper, deviceConfiguration);
        }
        private static async Task ListManagedDevices(IntuneHelper intuneHelper, string userPrincipalName)
        {
            var managedDevices = await intuneHelper.ListManagedDevicesForUser(userPrincipalName);

            Console.WriteLine($"Number of Intune managed devices for user {userPrincipalName}: {managedDevices.Count()}");
            if(managedDevices.Count() > 0)
            {
                Console.WriteLine(managedDevices.Select(x => $"-- {x.DeviceName} : {x.Manufacturer} {x.Model}").Aggregate((x, y) => $"{x}\n{y}"));
            }
        }

        private static async Task<WebApp> PublishWebApp(IntuneHelper intuneHelper, string url, string name, string publisher)
        {
            var webApp = await intuneHelper.PublishWebApp(url, name, publisher);

            Console.WriteLine($"Published web app: {webApp.Id}: {webApp.DisplayName} - {webApp.AppUrl}");

            return webApp;
        }

        private static async Task<DeviceConfiguration> CreateWindowsDeviceConfiguration(IntuneHelper intuneHelper, string displayName, string edgeHomePage, bool enableDeveloperMode)
        {
            var deviceConfiguration = await intuneHelper.CreateWindowsDeviceConfiguration(
                displayName,
                edgeHomePage,
                enableDeveloperMode);

            Console.WriteLine($"Created Device Configuration: {deviceConfiguration.Id}: {deviceConfiguration.DisplayName}");

            return deviceConfiguration;
        }

        private static async Task AssignAppToAllUsers(IntuneHelper intuneHelper, MobileApp app)
        {
            var assignments = await intuneHelper.AssignAppToAllUsers(app);
            Console.WriteLine($"App {app.DisplayName} has {assignments.Count()} assignments");
        }

        private static async Task AssignDeviceConfigurationToAllDevices(IntuneHelper intuneHelper, DeviceConfiguration deviceConfiguration)
        {
            var assignments = await intuneHelper.AssignDeviceConfigurationToAllDevices(deviceConfiguration);
            Console.WriteLine($"Device Configuration {deviceConfiguration.DisplayName} has {assignments.Count()} assignments");
        }

        private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            _graphServiceClient = new GraphServiceClient(authenticationProvider);
            return _graphServiceClient;
        }

        private static HttpClient GetAuthenticatedHTTPClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            _httpClient = new HttpClient(new AuthHandler(authenticationProvider, new HttpClientHandler()));
            return _httpClient;
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
                    string.IsNullOrEmpty(config["redirectUri"]) ||
                    string.IsNullOrEmpty(config["tenantId"]))
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
