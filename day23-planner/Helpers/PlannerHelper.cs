using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    // This class allows an implementation of IAuthenticationProvider to be inserted into the DelegatingHandler
    // pipeline of an HttpClient instance.  In future versions of GraphSDK, many cross-cutting concernts will
    // be implemented as DelegatingHandlers.  This AuthHandler will come in the box.
    public class PlannerHelper
    {
        private readonly GraphServiceClient _graphClient;
        public PlannerHelper(GraphServiceClient graphClient)
        {
            _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
        }
        public async Task PlannerHelperCall()
        {
            //Getting the first group we can find to create a plan
            var groupId = (await _graphClient.Me.GetMemberGroups(false).Request().PostAsync()).FirstOrDefault();

            if (groupId != null)
            {
                var users = await _graphClient.Users.Request(new List<QueryOption> {
                        new QueryOption("$top", "3")
                    }).GetAsync();

                var planId = await GetAndListCurrentPlans(groupId) ?? await CreatePlannerPlan(users, groupId);
                var bucketId = await CreatePlannerBucket(groupId, planId);
                await CreatePlannerTask(users, groupId, planId, bucketId);
            }
        }
        private async Task<string> GetAndListCurrentPlans(string groupId)
        {
            //Querying plans in current group
            var plans = await _graphClient.Groups[groupId].Planner.Plans.Request(new List<QueryOption>
            {
                new QueryOption("$orderby", "Title asc")
            }).GetAsync();
            if (plans.Any())
            {
                Console.WriteLine($"Number of plans in current tenant: {plans.Count}");
                Console.Write(plans.Select(x => $"-- {x.Title}").Aggregate((x, y) => $"{x}\n{y}"));
                return plans.First().Id;
            }
            else
            {
                Console.WriteLine("No existing plan");
                return null;
            }
        }
        private async Task<string> CreatePlannerPlan(IEnumerable<User> users, string groupId)
        {
            // Getting users to share the plan with
            var sharedWith = new PlannerUserIds();
            users.ToList().ForEach(x => sharedWith.Add(x.Id));

            // Creating a new planner plan
            var createdPlan = await _graphClient.Planner.Plans.Request().AddAsync(
                new PlannerPlan
                {
                    Title = $"My new Plan {Guid.NewGuid().ToString()}",
                    Owner = groupId,
                    Details = new PlannerPlanDetails
                    {
                        SharedWith = sharedWith,
                        CategoryDescriptions = new PlannerCategoryDescriptions
                        {
                            Category1 = "my first category",
                            Category2 = "my second category"
                        },
                    }
                }
            );
            Console.WriteLine($"Added a new plan {createdPlan.Id}");
            return createdPlan.Id;
        }
        private async Task<string> CreatePlannerBucket(string groupId, string planId)
        {
            // Creating a new bucket within the plan
            var createdBucket = await _graphClient.Planner.Buckets.Request().AddAsync(
                new PlannerBucket
                {
                    Name = "my first bucket",
                    OrderHint = " !",
                    PlanId = planId
                }
            );
            Console.WriteLine($"Added new bucket {createdBucket.Name} to plan");
            return createdBucket.Id;
        }
        private async Task CreatePlannerTask(IEnumerable<User> users, string groupId, string planId, string bucketId)
        {
            // Preparing the assignment for the task
            var assignments = new PlannerAssignments();
            users.ToList().ForEach(x => assignments.AddAssignee(x.Id));
            // Creating a task within the bucket
            var createdTask = await _graphClient.Planner.Tasks.Request().AddAsync(
                new PlannerTask
                {
                    DueDateTime = DateTimeOffset.UtcNow.AddDays(7),
                    Title = "Do the dishes",
                    Details = new PlannerTaskDetails
                    {
                        Description = "Do the dishes that are remaining in the sink"
                    },
                    Assignments = assignments,
                    PlanId = planId,
                    BucketId = bucketId
                }
            );
            Console.WriteLine($"Added new task {createdTask.Title} to bucket");
        }
    }
}