# Day 16 - Create a new User

- [Day 16 - Create a new User](#day-16-create-a-new-user)
    - [Prerequisites](#prerequisites)
    - [Step 1: Update the App Registration permissions](#step-1-update-the-app-rgistration-permissions)
    - [Step 2: Extend the app to create users](#step-2-extend-the-app-to-create-users)

## Prerequisites

To complete this sample you need the following:

- Complete the [Base Console Application Setup](../base-console-app/)
- [Visual Studio Code](https://code.visualstudio.com/) installed on your development machine. If you do not have Visual Studio Code, visit the previous link for download options. (**Note:** This tutorial was written with Visual Studio Code version 1.28.2. The steps in this guide may work with other versions, but that has not been tested.)
- [.Net Core SDK](https://www.microsoft.com/net/download/dotnet-core/2.1#sdk-2.1.403). (**Note** This tutorial was written with .Net Core SDK 2.1.403.  The steps in this guide may work with other versions, but that has not been tested.)
- [C# extension for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp)
- Either a personal Microsoft account with a mailbox on Outlook.com, or a Microsoft work or school account.

If you don't have a Microsoft account, there are a couple of options to get a free account:

- You can [sign up for a new personal Microsoft account](https://signup.live.com/signup?wa=wsignin1.0&rpsnv=12&ct=1454618383&rver=6.4.6456.0&wp=MBI_SSL_SHARED&wreply=https://mail.live.com/default.aspx&id=64855&cbcxt=mai&bk=1454618383&uiflavor=web&uaid=b213a65b4fdc484382b6622b3ecaa547&mkt=E-US&lc=1033&lic=1).
- You can [sign up for the Office 365 Developer Program](https://developer.microsoft.com/office/dev-program) to get a free Office 365 subscription.


## Step 1: Update the App Registration permissions

As this exercise requires new permissions the App Registration needs to be updated to include  the **User.ReadWrite.All** permission using the new Azure AD Portal App Registrations UI (in preview as of the time of publish Nov 2018).

1. Open a browser and navigate to the [Azure AD Portal](https://aad.portal.azure.com). Login using a **personal account** (aka: Microsoft Account) or **Work or School Account** with permissions to create app registrations.

    > **Note:** If you do not have permissions to create app registrations contact your Azure AD domain administrators.

1. Click **Azure Active Directory** from the left-hand navigation menu.

1. Click on the **.NET Core Graph Tutorial** item in the list

    > **Note:** If you used a different name while completing the [Base Console Application Setup](../base-console-app/) select that instead.

1. Click **API permissions** from the current blade content.

    1. Click **Add a permission** from the current blade content.
    1. On the **Request API permissions** flyout select **Microsoft Graph**.

        ![Screenshot of selecting Microsoft Graph permission to add to app registration](Images/aad-create-app-05.png)

    1. Select **Application permissions**.
    1. In the "Select permissions" search box type "User".
    1. Select **User.ReadWrite.All** from the filtered list.

        ![Screenshot of adding application permission for User.Read.All permission](Images/aad-create-app-06.png)

    1. Click **Add permissions** at the bottom of flyout.

1. Back on the API permissions content blade, click **Grant admin consent for \<name of tenant\>**.  
**need new screenshot here**
    ![Screenshot of granting admin consent for newly added permission](Images/aad-create-app-07.png)

    1. Click **Yes**.

## Step 2: Extend the app to create users

In this step you will add calls to the console application created in the [Base Console Application Setup](../base-console-app/) to provision a new user.

1. Inside the `Program` class add a new method `Build` with the following definition.  This method creates an instance of the `User` class with all required fields provided. This user will enableded and be required to change their password upon their next login.

    ```cs
    private static User BuildUserToAdd(string displayName, string alias, string domain, string password) 
    {
        var passwordProfile = new PasswordProfile
        {
            Password = password,
            ForceChangePasswordNextSignIn = true
        };
        var user = new User
        {
            DisplayName = displayName,
            UserPrincipalName = $@"{alias}@{domain}",
            MailNickname = alias,
            AccountEnabled = true,
            PasswordProfile = passwordProfile
        };
        return user;
    }
    ```
1. Continuing in the `Main` method add the following to pass the build the new user and then created in Azure Active Directory

    ```cs
    const string alias = "sdk_test";
    const string domain = "<tenant>.onmicrosoft.com";
    var userToAdd = BuildUserToAdd("SDK Test User", alias, domain, "ChangeThis!0");
    var added = graphClient.Users.Request().AddAsync(userToAdd).Result;
    Console.WriteLine("Graph SDK Add Result");
    Console.WriteLine(added.DisplayName);
    ```
    > **Important** the value supplied as the alias must be unique for your Azure Active Directory tenant and the domain parameter must match one of the domains associated with your Azure Active Directory tenant.

1. Continuing in the `Main` method add the following to query for the newly added user

    ```cs
    List<QueryOption> queryOptions = new List<QueryOption>
    {
        new QueryOption("$filter", $@"mailNickname eq '{alias}'")
    };

    var newUserResult = graphClient.Users.Request(queryOptions).GetAsync().Result;
    Console.WriteLine(newUserResult[0].DisplayName);
    Console.WriteLine(newUserResult[0].UserPrincipalName);
    ```

The console application is now able to provision new users into Azure Active Directory. In order to test the console application run the following commands from the command line:

```
dotnet build
dotnet run
```

After running this you have provisioned a new user into Azure Active Directory and are able to locate that newly added user account using an OData `$filter`. 