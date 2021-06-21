using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tehsat
{
    public class ServiceSocketManagement
    {
        public Dictionary<String,ServiceSocket> servicesockets = new Dictionary<String, ServiceSocket>();

        public Task<Dictionary<String, ServiceSocket>> GetServiceSockets()
        {
            return Task.FromResult(servicesockets);
        }

        public Task<ServiceSocket> GetServiceSocket(String id)
        {
            return Task.FromResult(servicesockets[id]);
        }

    }
}
