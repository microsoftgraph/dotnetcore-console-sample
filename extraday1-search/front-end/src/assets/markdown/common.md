  ```csharp
  GraphServiceClient graphClient = new GraphServiceClient( authProvider );

  SearchRequestObject searchRequestObject = new SearchRequestObject
{
      EntityTypes = new List<EntityType>()
      {
        EntityType.DriveItem,
        EntityType.List
        //EntityType.ListItem,
        //EntityType.Drive,
        //EntityType.Site,
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