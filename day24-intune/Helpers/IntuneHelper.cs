using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;

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

        /// <summary>
        /// Get the list of Intune managed devices for a user.
        /// </summary>
        /// <param name="userPrincipalName">The user principal name (UPN) for the user</param>
        /// <returns>The Intune managed devices.</returns>
        public async Task<ICollection<ManagedDevice>> ListManagedDevicesForUser(string userPrincipalName)
        {
            List<ManagedDevice> managedDevices = new List<ManagedDevice>();
            List<QueryOption> queryOptions = new List<QueryOption>
            {
                new QueryOption("$orderby", "deviceName")
            };

            var deviceResults = await _graphClient.Users[userPrincipalName].ManagedDevices.Request(queryOptions).GetAsync();

            managedDevices.AddRange(deviceResults.CurrentPage);

            // Page through the results in case there are more than one page of devices.
            while (deviceResults.NextPageRequest != null)
            {
                deviceResults = await deviceResults.NextPageRequest.GetAsync();
                managedDevices.AddRange(deviceResults.CurrentPage);
            }

            return managedDevices;
        }

        /// <summary>
        /// Publish a web app to Intune
        /// </summary>
        /// <param name="url">The url for the website</param>
        /// <param name="name">The name of the website</param>
        /// <param name="publisher">The name of the publisher</param>
        /// <returns>The created app</returns>
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

        /// <summary>
        /// Assign an Intune MobileApp to all users.
        /// </summary>
        /// <param name="app">The application to assign.</param>
        /// <returns>The assignements for the app.</returns>
        public async Task<ICollection<MobileAppAssignment>> AssignAppToAllUsers(MobileApp app)
        {
            var assignment = BuildMobileAppAssignment();

            await _graphClient.DeviceAppManagement.MobileApps[app.Id].Assign(new[] { assignment }).Request().PostAsync();

            return await _graphClient.DeviceAppManagement.MobileApps[app.Id].Assignments.Request().GetAsync();
        }

        /// <summary>
        /// Create a Windows 10 Device Configuration in Intune
        /// </summary>
        /// <param name="displayName">The display name of the device configuration.</param>
        /// <param name="edgeHomePage">The homepage to show in Edge</param>
        /// <param name="enableDeveloperMode">Enable developer mode on the device.</param>
        /// <returns>The created device configuraton</returns>
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

        /// <summary>
        /// Assigns an Intune Device Configuration to all Intune managed devices
        /// </summary>
        /// <param name="deviceConfiguration">The device configuration.</param>
        /// <returns>The assignments for the device configuration.</returns>
        public async Task<ICollection<DeviceConfigurationAssignment>> AssignDeviceConfigurationToAllDevices(DeviceConfiguration deviceConfiguration)
        {
            var assignment = BuildDeviceConfigurationAssignment();

            await _graphClient.DeviceManagement.DeviceConfigurations[deviceConfiguration.Id].Assign(new[] { assignment }).Request().PostAsync();

            return await _graphClient.DeviceManagement.DeviceConfigurations[deviceConfiguration.Id].Assignments.Request().GetAsync();
        }

        /// <summary>
        /// Create a mobile app assignement for all users.
        /// </summary>
        /// <returns>The mobile app assignment</returns>
        private static MobileAppAssignment BuildMobileAppAssignment()
        {
            return new MobileAppAssignment
            {
                Intent = InstallIntent.Available,
                Target = new AllLicensedUsersAssignmentTarget()
            };
        }

        /// <summary>
        /// Create a device configuration assignment for all devices
        /// </summary>
        /// <returns>The device configuration assignment</returns>
        private static DeviceConfigurationAssignment BuildDeviceConfigurationAssignment()
        {
            return new DeviceConfigurationAssignment
            {
                Target = new AllDevicesAssignmentTarget()
            };
        }
    }
}