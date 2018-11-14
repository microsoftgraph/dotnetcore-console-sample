# Day NN - Getting your Team organized with Planner through the Microsoft Graph

> When adding Images, save them in the Images folder for your day

- [Day NN - Getting your Team organized with Planner through the Microsoft Graph](#day-nn-scenario-template)
    - [Prerequisites](#prerequisites)
    - [Step 1: Update the App Registration permissions](#step-1-update-the-app-rgistration-permissions)
    - [Step 2: Extend the app to yyy](#step-2-extend-the-app-to-yyy)
        - [Create the MyHelper class](#create-the-myhelper-class)
        - [Extend program to yyy](#extend-program-to-yyy)

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

As this exercise requires new permissions the App Registration needs to be updated to include the **Group.ReadWrite.All** permission using the new Azure AD Portal App Registrations UI (in preview as of the time of publish Nov 2018).

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
    1. In the "Select permissions" search box type "\<Start of permission string\>".
    1. Select **Group.ReadWrite.All** from the filtered list.

        ![Screenshot of adding application permission for Group.ReadWrite.All permission](Images/aad-add-planner-permissions.png)

    1. Click **Add permissions** at the bottom of flyout.

1. Back on the API permissions content blade, click **Grant admin consent for \<name of tenant\>**.  
**need new screenshot here**
    ![Screenshot of granting admin consent for newly added permission](Images/aad-grant-permissions-planner.png)

    1. Click **Yes**.

## Step 2: Extend the app to List existing Planner Plans

In this step you will create a Helper method that encapsulates the logic for listing existing plans and then add calls to the console application created in the [Base Console Application Setup](../base-console-app/).

### Extend program to List existing Planner Plans

1. Inside the `Program` class add a new method `ListCurrentPlans` with the following definition.  This method lists all the current plans in the tenant.

    ```cs
    private static async Task ListCurrentPlans(GraphServiceClient graphClient) {
        //Querying plans in current tenant
        var plans = await graphClient.Planner.Plans.Request(new List<QueryOption>
        {
            new QueryOption("$orderby", "Title asc")
        }).GetAsync();
        Console.WriteLine($"Number of plans in current tenant: {plans.Count}");
        Console.Write(plans.Select(x => $"-- {x.Title}").Aggregate((x,y) => $"{x}\n{y}"));
    }
    ```
1. Inside the `Program` class add the main helper method `PlannerHelperCall` with the following definition.
We will build on this method during the exercice.

    ```cs
    private static async Task PlannerHelperCall(IConfigurationRoot config) {
        //Query using Graph SDK (preferred when possible)
        var graphClient = GetAuthenticatedGraphClient(config);

        await ListCurrentPlans(graphClient);
    }
    ```

1. Continuing in the `Main` method add the following code to call the new method.

    ```cs
    PlannerHelperCall(config).GetAwaiter().GetResult();
    ```
1. Above the `Program` class, add a reference to Linq adding this line.

    ```cs
    using System.Linq;
    ```
1. Save all files.

The console application is now able to list the plans in the tenant. In order to test the console application run the following commands from the command line:

```
dotnet build
dotnet run
```

### Extend program to Create a plan

1. Inside the `Program` class add a new method `CreatePlannerPlan` with the following definition.  This method create a new plan in the tenant.

    ```cs
    private static async Task<string> CreatePlannerPlan(GraphServiceClient graphClient, IEnumerable<User> users, string groupId) {
        // Getting users to share the plan with
        var sharedWith = new PlannerUserIds();
        users.ToList().ForEach(x => sharedWith.Add(x.Id));

        // Creating a new planner plan
        var createdPlan = await graphClient.Planner.Plans.Request().AddAsync(
            new PlannerPlan {
                Title = $"My new Plan {Guid.NewGuid().ToString()}",
                Owner = groupId,
                Details = new PlannerPlanDetails {
                    SharedWith = sharedWith,
                    CategoryDescriptions = new PlannerCategoryDescriptions {
                        Category1 = "my first category",
                        Category2 = "my second category"
                    },
                }
            }
        );
        Console.WriteLine($"Added a new plan {createdPlan.Id}");
        return createdPlan.Id;
    }
    ```
1. Inside the `Program` class update the main helper method `PlannerHelperCall` with the following definition.

    ```cs
    private static async Task PlannerHelperCall(IConfigurationRoot config) {
            //Query using Graph SDK (preferred when possible)
            var graphClient = GetAuthenticatedGraphClient(config);

            await ListCurrentPlans(graphClient);

            //Getting the first group we can find to create a plan
            var group = (await graphClient.Groups.Request(new List<QueryOption> {
                new QueryOption("$top", "1")
            }).GetAsync()).FirstOrDefault();

            if(group != null) {
                var users = await graphClient.Users.Request(new List<QueryOption> {
                        new QueryOption("$top", "3")
                    }).GetAsync();
                
                var planId = await CreatePlannerPlan(graphClient, users, group.Id);
            }
        }
    ```
1. Save all files.

The console application is now able add new plans in the tenant. In order to test the console application run the following commands from the command line:

```
dotnet build
dotnet run
```

### Extend program to add a Bucket

1. Inside the `Program` class add a new method `CreatePlannerBucket` with the following definition. This method adds a new bucket to a plan.

    ```cs
    private static async Task<string> CreatePlannerBucket(GraphServiceClient graphClient, string planId) {
        // Creating a new bucket within the plan
        var createdBucket = await graphClient.Planner.Plans[planId].Buckets.Request().AddAsync(
            new PlannerBucket {
                Name = "my first bucket",
                OrderHint = 1.ToString()
            }
        );
        Console.WriteLine($"Added new bucket {createdBucket.Name} to plan");
        return createdBucket.Id;
    }
    ```
1. Inside the `Program` class update the main helper method `PlannerHelperCall` with the following definition.

    ```cs
    private static async Task PlannerHelperCall(IConfigurationRoot config) {
            //Query using Graph SDK (preferred when possible)
            var graphClient = GetAuthenticatedGraphClient(config);

            await ListCurrentPlans(graphClient);

            //Getting the first group we can find to create a plan
            var group = (await graphClient.Groups.Request(new List<QueryOption> {
                new QueryOption("$top", "1")
            }).GetAsync()).FirstOrDefault();

            if(group != null) {
                var users = await graphClient.Users.Request(new List<QueryOption> {
                        new QueryOption("$top", "3")
                    }).GetAsync();
                
                var planId = await CreatePlannerPlan(graphClient, users, group.Id);
                var bucketId = await CreatePlannerBucket(graphClient, planId);
            }
        }
    ```
1. Save all files.

The console application is now able add new buckets to a plan. In order to test the console application run the following commands from the command line:

```
dotnet build
dotnet run
```

### Extend program to add a Task

1. Inside the `Program` class add a new method `CreatePlannerTask` with the following definition. This method adds a new task to a bucket.

    ```cs
    private static async Task CreatePlannerTask(GraphServiceClient graphClient, IEnumerable<User> users, string planId, string bucketId){
        // Preparing the assignment for the task
        var assignments = new PlannerAssignments ();
        users.ToList().ForEach( x=> assignments.AddAssignee(x.Id));
        // Creating a task within the bucket
        var createdTask = await graphClient.Planner.Plans[planId].Buckets[bucketId].Tasks.Request().AddAsync(
            new PlannerTask {
                DueDateTime = DateTimeOffset.UtcNow.AddDays(7),
                Title = "Do the dishes",
                Details = new PlannerTaskDetails {
                    Description = "Do the dishes that are remaining in the sink"
                },
                Assignments = assignments
            }
        );
        Console.WriteLine($"Added new task {createdTask.Title} to bucket");
    }
    ```
1. Inside the `Program` class update the main helper method `PlannerHelperCall` with the following definition.

    ```cs
    private static async Task PlannerHelperCall(IConfigurationRoot config) {
            //Query using Graph SDK (preferred when possible)
            var graphClient = GetAuthenticatedGraphClient(config);

            await ListCurrentPlans(graphClient);

            //Getting the first group we can find to create a plan
            var group = (await graphClient.Groups.Request(new List<QueryOption> {
                new QueryOption("$top", "1")
            }).GetAsync()).FirstOrDefault();

            if(group != null) {
                var users = await graphClient.Users.Request(new List<QueryOption> {
                        new QueryOption("$top", "3")
                    }).GetAsync();
                
                var planId = await CreatePlannerPlan(graphClient, users, group.Id);
                var bucketId = await CreatePlannerBucket(graphClient, planId);
                await CreatePlannerTask(graphClient, users, planId, bucketId);
            }
        }
    ```
1. Save all files.

The console application is now able add new tasks to a bucket. In order to test the console application run the following commands from the command line:

```
dotnet build
dotnet run
```

