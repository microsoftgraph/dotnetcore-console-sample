  GraphServiceClient graphClient = new GraphServiceClient( authProvider );

  var requests = new List()
  {
    new SearchRequestObject
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
    }
  };
  
  await graphClient.Search
    .Query(requests,null)
    .Request()
    .PostAsync();