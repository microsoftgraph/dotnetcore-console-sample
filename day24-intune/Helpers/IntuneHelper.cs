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
    public class IntuneHelper
    {
        private GraphServiceClient _graphClient;

        public IntuneHelper(GraphServiceClient graphClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
            _graphClient = graphClient;
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
            var assignment = BuildMobileAppAssignment();

            await _graphClient.DeviceAppManagement.MobileApps[app.Id].Assign(new[] { assignment }).Request().PostAsync();

            return await _graphClient.DeviceAppManagement.MobileApps[app.Id].Assignments.Request().GetAsync();
        }

        public async Task<ICollection<DeviceConfigurationAssignment>> AssignDeviceConfigurationToAllDevices(DeviceConfiguration deviceConfiguration)
        {
            var assignment = BuildDeviceConfigurationAssignment();

            await _graphClient.DeviceManagement.DeviceConfigurations[deviceConfiguration.Id].Assign(new[] { assignment }).Request().PostAsync();

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

        private static MobileAppAssignment BuildMobileAppAssignment()
        {
            return new MobileAppAssignment
            {
                Intent = InstallIntent.Available,
                Target = new AllLicensedUsersAssignmentTarget()
            };
        }

        private static DeviceConfigurationAssignment BuildDeviceConfigurationAssignment()
        {
            return new DeviceConfigurationAssignment
            {
                Target = new AllDevicesAssignmentTarget()
            };
        }
    }
}