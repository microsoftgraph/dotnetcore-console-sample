using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;

namespace ConsoleGraphTest
{
    /**
     * Please rename your helper class to match it's purpose
     */
    public class IntuneHelper
    {

        /**
         * Inject either a GraphServiceClient or an HttpClient (with Authentiation supplied)
         * Which you choose to use will depend on your scenario but the GraphServiceClient should be used where practical
         * Please delete the constructor you don't use
         */
        private GraphServiceClient _graphClient;
        private HttpClient _httpClient;
        public IntuneHelper(GraphServiceClient graphClient, HttpClient httpClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
            _graphClient = graphClient;
            if (null == httpClient) throw new ArgumentNullException(nameof(httpClient));
            _httpClient = httpClient;
        }

        // Add Public methods to provide functionality for your scenario.

        public async Task<WebApp> PublishWebApp(string url, string name, string publisher)
        {
            var webApp = new WebApp
            {
                AppUrl = url,
                DisplayName = name,
                Publisher = publisher
            };

            return await _graphClient.DeviceAppManagement.MobileApps.Request().AddAsync(webApp) as WebApp;
        }

        public async Task<ICollection<MobileAppAssignment>> AssignAppToAllUsers(MobileApp app)
        {
            var assignment = BuildAppAssignmentRequestJson();

            string requestUrl = _graphClient.BaseUrl + $"/deviceAppManagement/mobileApps/{app.Id}/assign";

            var response = await _httpClient.PostAsync(requestUrl, new StringContent(assignment.ToString(), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return await _graphClient.DeviceAppManagement.MobileApps[app.Id].Assignments.Request().GetAsync();
        }

        public async Task<ICollection<DeviceConfigurationAssignment>> AssignDeviceConfigurationToAllDevices(DeviceConfiguration deviceConfiguration)
        {
            var assignment = BuildDeviceConfigurationAssignmentRequestJson();



            var response = await _httpClient.PostAsync(
                _graphClient.DeviceManagement.DeviceConfigurations[deviceConfiguration.Id].Assign().Request().RequestUrl,
                new StringContent(assignment.ToString(), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return await _graphClient.DeviceManagement.DeviceConfigurations[deviceConfiguration.Id].Assignments.Request().GetAsync();
        }

        public async Task<ICollection<ManagedDevice>> ListManagedDevicesForUser(string userPrincipalName)
        {
            List<ManagedDevice> managedDevices = new List<ManagedDevice>();
            List<QueryOption> queryOptions = new List<QueryOption>
            {
                new QueryOption("$orderby", "deviceName")
            };

            var deviceResults = await _graphClient.Users[userPrincipalName].ManagedDevices.Request(queryOptions).GetAsync();

            managedDevices.AddRange(deviceResults.CurrentPage);
            while (deviceResults.NextPageRequest != null)
            {
                deviceResults = await deviceResults.NextPageRequest.GetAsync();
                managedDevices.AddRange(deviceResults.CurrentPage);
            } 

            return managedDevices;
        }

        public async Task<DeviceConfiguration> CreateWindowsDeviceConfiguration(string displayName, string edgeHomePage, bool enableDeveloperMode)
        {
            var deviceConfiguration = new Windows10GeneralConfiguration
            {
                DisplayName = displayName,
                EdgeHomepageUrls = new[] { edgeHomePage },
                DeveloperUnlockSetting = enableDeveloperMode ? StateManagementSetting.Allowed : StateManagementSetting.Blocked
            };

            return await _graphClient.DeviceManagement.DeviceConfigurations.Request().AddAsync(deviceConfiguration);
        }

        // Add private methods to encapsulate housekeeping work away from public methods
        private static JObject BuildAppAssignmentRequestJson()
        {
            return new JObject
            {
                new JProperty(
                    "mobileAppAssignments",
                    new JArray(
                        new JObject(
                            new JProperty(
                                "@odata.type",
                                "microsoft.graph.mobileAppAssignment"),
                            new JProperty(
                                "intent", 
                                "availableWithoutEnrollment"),
                            new JProperty(
                                "target", 
                                new JObject(
                                    new JProperty(
                                        "@odata.type", 
                                        "microsoft.graph.allLicensedUsersAssignmentTarget"))))))
            };
        }

        private static JObject BuildDeviceConfigurationAssignmentRequestJson()
        {
            return new JObject
            {
                new JProperty(
                    "assignments",
                    new JArray(
                        new JObject(
                            new JProperty(
                                "@odata.type",
                                "microsoft.graph.deviceConfigurationAssignment"),
                            new JProperty(
                                "target",
                                new JObject(
                                    new JProperty(
                                        "@odata.type",
                                        "microsoft.graph.allDevicesAssignmentTarget"))))))
            };
        }
    }
}