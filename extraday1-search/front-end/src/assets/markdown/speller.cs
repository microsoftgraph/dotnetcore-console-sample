GraphServiceClient graphClient = new GraphServiceClient(authProvider);

var requests = new List()
  {
    SearchAlterationOptions searchAlterationOptions = new SearchAlterationOptions()
    {
        // Set it as false, will not return results when service get results from corrected word
        EnableModification = true, 
        // Return the suggestion corrected search term
        EnableSuggestion = true
    };

new SearchRequestObject
{
  EntityTypes = new List<EntityType>()
{
        EntityType.DriveItem, // sample entity types, not only support them
        EntityType.List,
},
  Query = new SearchQuery
  {
    QueryString = "informatino" // wrong search term with no results
  },
}
  };

await graphClient.Search
  .Query(requests, searchAlterationOptions)
  .Request()
  .PostAsync();