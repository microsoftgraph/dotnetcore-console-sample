# dotnetcore-console-sample

## NOTE

This repo is a work in progress and as such does not have a complete README at this time.  Once ready there will be instructions similar to the [ASP.Net Getting Started lab](https://github.com/microsoftgraph/msgraph-training-aspnetmvcapp/blob/master/Lab.md) for registering an Azure AD application, granting permissions, etc.  For the time being do not treat this as a complete sample.  Thank you for your patience.

- [dotnetcore-console-sample](#dotnetcore-console-sample)
    - [NOTE](#note)
    - [Prerequisites](#prerequisites)
    - [Step 1: Create a .Net Core Console Application](#step-1-create-a-net-core-console-application)
    - [Step 2: Register a web application with the Application Registration Portal](#step-2-register-a-web-application-with-the-application-registration-portal)
    - [Step 3: Extend the app for Azure AD Authentication](#step-3-extend-the-app-for-azure-ad-authentication)
    - [Step 4: Extend the app for Microsoft Graph](#step-4-extend-the-app-for-microsoft-graph)
        - [Get user information from tenant](#get-user-information-from-tenant)
    - [Contributing](#contributing)

## Prerequisites

To complete this sample you need the following:

- [Visual Studio Code](https://code.visualstudio.com/) installed on your development machine. If you do not have Visual Studio Code, visit the previous link for download options. (**Note:** This tutorial was written with Visual Studio Code version 1.28.2. The steps in this guide may work with other versions, but that has not been tested.)
- Either a personal Microsoft account with a mailbox on Outlook.com, or a Microsoft work or school account.

If you don't have a Microsoft account, there are a couple of options to get a free account:

- You can [sign up for a new personal Microsoft account](https://signup.live.com/signup?wa=wsignin1.0&rpsnv=12&ct=1454618383&rver=6.4.6456.0&wp=MBI_SSL_SHARED&wreply=https://mail.live.com/default.aspx&id=64855&cbcxt=mai&bk=1454618383&uiflavor=web&uaid=b213a65b4fdc484382b6622b3ecaa547&mkt=E-US&lc=1033&lic=1).
- You can [sign up for the Office 365 Developer Program](https://developer.microsoft.com/office/dev-program) to get a free Office 365 subscription.

## Step 1: Create a .Net Core Console Application

Create a folder for the console application.  Open the command line and navigate to this folder.  Run the following command:

```
dotnet new console
```

Before moving on, install the following NuGet packages that you will use later.

- Microsoft.Identity.Client
- Microsoft.Graph
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.FileExtensions
- Microsoft.Extensions.Configuration.Json

Run the following commands to install these NuGet packages:

```
dotnet add package Microsoft.Identity.Client
dotnet add package Microsoft.Graph
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.FileExtensions
dotnet add package Microsoft.Extensions.Configuration.Json
```

## Step 2: Register a web application with the Application Registration Portal

In this exercise, you will create a new Azure AD web application registration using the Application Registry Portal (ARP).

1. Open a browser and navigate to the [Application Registration Portal](https://apps.dev.microsoft.com). Login using a **personal account** (aka: Microsoft Account) or **Work or School Account**.

1. Select **Add an app** at the top of the page.

    > **Note:** If you see more than one **Add an app** button on the page, select the one that corresponds to the **Converged apps** list.

1. On the **Register your application** page, set the **Application Name** to **.NET Core Graph Tutorial** and select **Create**.

    ![Screenshot of creating a new app in the App Registration Portal website](Images/arp-create-app-01.png)

1. On the **.NET Core Graph Tutorial Registration** page, under the **Properties** section, copy the **Application Id** as you will need it later.

    ![Screenshot of newly created application's ID](Images/arp-create-app-02.png)

1. Scroll down to the **Application Secrets** section.

    1. Select **Generate New Password**.
    1. In the **New password generated** dialog, copy the contents of the box as you will need it later.

        > **Important:** This password is never shown again, so make sure you copy it now.

    ![Screenshot of newly created application's password](Images/arp-create-app-03.png)

1. Scroll down to the **Platforms** section.

    1. Select **Add Platform**.
    1. In the **Add Platform** dialog, select **Web**.

        ![Screenshot creating a platform for the app](Images/arp-create-app-04.png)

    1. In the **Web** platform box, enter a URL you copied from the Visual Studio project's properties for the **Redirect URLs**.

        ![Screenshot of the newly added Web platform for the application](Images/arp-create-app-05.png)

1. Scroll to the bottom of the page and select **Save**.

## Step 3: Extend the app for Azure AD Authentication

In this step you will extend the application from the previous step to support authentication with Azure AD. This is required to obtain the necessary OAuth access token to call the Microsoft Graph. In this step you will integrate the [Microsoft Authentication Library](https://www.nuget.org/packages/Microsoft.Identity.Client/) library into the application.

1. Rename the `appsettings.json.example` file to `appsettings.json`.
1. Edit `appsettings.json`:
    1. Replace `YOUR_APP_ID_HERE` with your application ID from the App Registration Portal.
    2. Replace `YOUR_APP_SECRET_HERE` with your application password from the App Registration Portal.
    3. Replace `YOUR_TENANT_ID_HERE` with your tenant ID.

> **Important:** If you're using source control such as git, now would be a good time to exclude the `appsettings.json` file from source control to avoid inadvertently leaking your app ID and secret.

## Step 4: Extend the app for Microsoft Graph

In this step you will incorporate the Microsoft Graph into the application. For this application, you will use the [Microsoft Graph Client Library for .NET](https://github.com/microsoftgraph/msgraph-sdk-dotnet) to make calls to Microsoft Graph.

### Get user information from tenant

Start by opening the `Program.cs` file.  Add the following "using" statements to the top of the file.

```cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
```

Inside the `Program` class add static references to the `GraphServiceClient`.  This static variable will be used to instantiate the client used to make calls against the Microsoft Graph.

```cs
private static GraphServiceClient _graphServiceClient;
```

Add a new method `LoadAppSettings` with the following definition.  This method retrieves the configuration values from a separate file.  This allows updating the configuration (client Id, client secret, etc. independently of the code itself.)  This is a general best practice when possible to separate configuration from code.

```cs
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
```

Add a new method `GetAuthenticatedGraphClient` with the following definition.  This method creates an instance of the `GraphServiceClient` from the static reference.  The `GraphServiceClient` instance uses the configuration returned from previous method.

```cs
private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
{
    var clientId = config["applicationId"];
    var clientSecret = config["applicationSecret"];
    var redirectUri = config["redirectUri"];
    var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";

    List<string> scopes = new List<string>();
    scopes.Add("https://graph.microsoft.com/.default");

    var cca = new ConfidentialClientApplication(clientId, authority, redirectUri, new ClientCredential(clientSecret), null, null);
    var authResult = cca.AcquireTokenForClientAsync(scopes).Result;

    _graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
    {
        requestMessage
          .Headers
          .Authorization = new AuthenticationHeaderValue("bearer", authResult.AccessToken);

        return Task.FromResult(0);
    }));

    return _graphServiceClient;
}
```

Inside the `Main` method add the following to load the configuration settings.

```cs
var config = LoadAppSettings();
if (null == config)
{
    Console.WriteLine("Missing or invalid appsettings.json file. Please see README.md for configuration instructions.");
    return;
}
```

Continuing in the `Main` method add the following to get an authenticated instance of the `GraphServiceClient` and send a request to retrieve the first user from Users endpoint on the Microsoft Graph.

```cs
GraphServiceClient graphClient = GetAuthenticatedGraphClient(config);
List<QueryOption> options = new List<QueryOption>
{
    new QueryOption("$top", "1")
};

var graphResult = graphClient.Users.Request(options).GetAsync().Result;
Console.WriteLine(graphResult);
```

This completes all file edits and additions.  Ensure all files are saved.  Run the following commands from the command line:

```
dotnet build
dotnet run
```

Consider what this code is doing.

- The `GetAuthenticatedGraphClient` function initializes a `GraphServiceClient` with an authentication provider that calls `AcquireTokenForClientAsync`.
- In the `Main` function:
  - The URL that will be called is `/v1.0/users/$top=1`.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
