using System.Collections.Generic;
using System.Linq;
using Microsoft.Graph;

namespace GraphWebhooks
{
    public interface ISubscriptionRepository
    {
        void Save(Subscription subscription);
        void Delete(string id);
        Subscription Load(string id);
        Subscription LoadByUpn(string upn);
    }

    public class SubscriptionRepository : ISubscriptionRepository
    {
        private IList<Subscription> _subscriptions;

        public SubscriptionRepository()
        {
            _subscriptions = new List<Subscription>();
        }

        public void Save(Subscription subscription) => _subscriptions.Add(subscription);

        public void Delete(string id)
        {
            var toDelete = Load(id);
            _subscriptions.Remove(toDelete);
        }

        public Subscription Load(string id) => _subscriptions.FirstOrDefault(s => s.Id == id);

        public Subscription LoadByUpn(string upn) => _subscriptions.FirstOrDefault(s => s.Resource.Contains($"/{upn}/"));
    }
}
