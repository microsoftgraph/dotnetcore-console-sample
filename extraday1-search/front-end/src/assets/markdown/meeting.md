```csharp
GraphServiceClient graphClient = new GraphServiceClient( authProvider );

SearchRequestObject searchRequestObject = new SearchRequestObject
{
      EntityTypes = new List<EntityType>()
      {
        EntityType.Event
      },
      Query = new SearchQuery
      {
        QueryString = "*"
      }
};


var requests = new List<SearchRequestObject>() { searchRequestObject };
  
await graphClient.Search
    .Query(requests,null)
    .Request()
    .PostAsync();
  ```