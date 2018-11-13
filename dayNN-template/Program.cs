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
            PlannerHelperCall(config).GetAwaiter().GetResult();
        }
        private static async Task PlannerHelperCall(IConfigurationRoot config) {
            //Query using Graph SDK (preferred when possible)
            var graphClient = GetAuthenticatedGraphClient(config);

            //Getting the first group we can find to create a plan
            var groupId = (await graphClient.Me.GetMemberGroups(false).Request().PostAsync()).FirstOrDefault();

            if(groupId != null) {
                var users = await graphClient.Users.Request(new List<QueryOption> {
                        new QueryOption("$top", "3")
                    }).GetAsync();
                
                var planId = await GetAndListCurrentPlans(graphClient, groupId) ?? await CreatePlannerPlan(graphClient, users, groupId);
                var bucketId = await CreatePlannerBucket(graphClient, groupId, planId);
                await CreatePlannerTask(graphClient, users, groupId, planId, bucketId);
            }
        }
        private static async Task<string> GetAndListCurrentPlans(GraphServiceClient graphClient, string groupId) {
            //Querying plans in current group
            var plans = await graphClient.Groups[groupId].Planner.Plans.Request(new List<QueryOption>
            {
                new QueryOption("$orderby", "Title asc")
            }).GetAsync();
            if (plans.Any())
            {
                Console.WriteLine($"Number of plans in current tenant: {plans.Count}");
                Console.Write(plans.Select(x => $"-- {x.Title}").Aggregate((x, y) => $"{x}\n{y}"));
                return plans.First().Id;
            } else
            {
                Console.WriteLine("No existing plan");
                return string.Empty;
            }
        }
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
        private static async Task<string> CreatePlannerBucket(GraphServiceClient graphClient, string groupId, string planId) {
            // Creating a new bucket within the plan
            var createdBucket = await graphClient.Planner.Buckets.Request().AddAsync(
                new PlannerBucket {
                    Name = "my first bucket",
                    OrderHint = " !",
                    PlanId = planId
                }
            );
            Console.WriteLine($"Added new bucket {createdBucket.Name} to plan");
            return createdBucket.Id;
        }
        private static async Task CreatePlannerTask(GraphServiceClient graphClient, IEnumerable<User> users, string groupId, string planId, string bucketId){
            // Preparing the assignment for the task
            var assignments = new PlannerAssignments ();
            users.ToList().ForEach( x=> assignments.AddAssignee(x.Id));
            // Creating a task within the bucket
            var createdTask = await graphClient.Planner.Tasks.Request().AddAsync(
                new PlannerTask {
                    DueDateTime = DateTimeOffset.UtcNow.AddDays(7),
                    Title = "Do the dishes",
                    Details = new PlannerTaskDetails {
                        Description = "Do the dishes that are remaining in the sink"
                    },
                    Assignments = assignments,
                    PlanId = planId,
                    BucketId = bucketId
                }
            );
            Console.WriteLine($"Added new task {createdTask.Title} to bucket");
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
            var clientSecret = config["applicationSecret"];
            var redirectUri = config["redirectUri"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}";

            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = new PublicClientApplication(clientId, authority);
            return new DeviceCodeFlowAuthorizationProvider(cca, scopes);
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
