```csharp
GraphServiceClient graphClient = new GraphServiceClient(authProvider);

SearchRequestObject searchRequestObject = new SearchRequestObject
{
  EntityTypes = new List<EntityType>()
      {
        EntityType.DriveItem
      },
  Query = new SearchQuery
  {
    QueryString = "*"
  },
  SortProperties = new List()
      {
        new SortProperty
        {
          Name = "lastModifiedDateTime",
          IsDescending = true
        }
      }
};


var requests = new List<SearchRequestObject>() { searchRequestObject };

await graphClient.Search
  .Query(requests, null)
  .Request()
  .PostAsync();
  ```