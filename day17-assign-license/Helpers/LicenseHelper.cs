using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    public class LicenseHelper
    {
        private GraphServiceClient _graphClient;
        
        public LicenseHelper(GraphServiceClient graphClient)
        {
            if (null == graphClient) throw new ArgumentNullException(nameof(graphClient));
            _graphClient = graphClient;
        }

        public async Task<User> GetUser(string UPN)
        {
            var user = await _graphClient.Users[UPN].Request().GetAsync();
            return user;
        }

        public async Task<SubscribedSku> GetLicense()
        {
            var skuResult = await _graphClient.SubscribedSkus.Request().GetAsync();
            return skuResult[0];
        }

        public async Task AddLicense(string userId, Guid? skuId)
        {
            var licensesToAdd = new List<AssignedLicense>();
            var licensesToRemove = new List<Guid>();

            var license = new AssignedLicense()
            {
                SkuId = skuId,
            };

            licensesToAdd.Add(license);

            await _graphClient.Users[userId].AssignLicense(licensesToAdd, licensesToRemove).Request().PostAsync();
        }
    }
}