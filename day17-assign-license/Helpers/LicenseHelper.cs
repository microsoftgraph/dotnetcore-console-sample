using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace ConsoleGraphTest
{
    /**
     * Please rename your helper class to match it's purpose
     */
    public class LicenseHelper
    {

        /**
         * Inject either a GraphServiceClient or an HttpClient (with Authentiation supplied)
         * Which you choose to use will depend on your scenario but the GraphServiceClient should be used where practical
         * Please delete the constructor you don't use
         */
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