  GraphServiceClient graphClient = new GraphServiceClient( authProvider );

  var requests = new List()
  {
    new SearchRequestObject
    {
      EntityTypes = new List<EntityType>()
      {
        EntityType.Event,
      },
      Query = new SearchQuery
      {
        QueryString = "*"
      },
    }
  };
  
  await graphClient.Search
    .Query(requests,null)
    .Request()
    .PostAsync();