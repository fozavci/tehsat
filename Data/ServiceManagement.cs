using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tehsat
{
    public class ServiceManagement
    {
        public Dictionary<String,Service> services = new Dictionary<String,Service>();

        public Task<Dictionary<String, Service>> GetServices()
        {
            return Task.FromResult(services);
        }

        public Task<Service> GetService(String id)
        {
            return Task.FromResult(services[id]);
        }

        public Service EmptyService(String name, Profile templateprofile)
        {
            string id = Common.RandomStringGenerator(16);

            Profile profile = templateprofile.Clone();

            // Creating a service for the profile
            Service newservice = new Service()
            {
                Id = id,
                Name = name,
                Profile = profile,
                ChannelObject = profile.ChannelObject,
                ChannelType = profile.ChannelType,
                SocketObject = new ServiceSocket(profile.ChannelObject)
            };
            newservice.ChannelObject.ProfileId = profile.Id;
            newservice.ChannelObject.Protocol = profile.ChannelType;

            return newservice;
        }

        public Task<bool> AddService(Service newservice)
        {
            if (!services.ContainsKey(newservice.Id))
            {
                // Creating a socket object for the profile
                newservice.UpdateSocketObject();

                // Adding the service to the services
                services.TryAdd(newservice.Id, newservice);                
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateService(Service newservice)
        {
            if (!services.ContainsKey(newservice.Id))
            {
                // Creating a socket object for the profile
                newservice.UpdateSocketObject();

                // Updating the service to the services
                services[newservice.Id] = newservice;

                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
        



        public Task<bool> DeleteService(String id)
        {

            if (services.ContainsKey(id))
            {
                services.Remove(id);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        
        }

    }
}
