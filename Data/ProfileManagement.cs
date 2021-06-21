using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tehsat
{
    public class ProfileManagement
    {
        public static Dictionary<String, Profile> profiles = new Dictionary<String, Profile>();
        
        public Task<Dictionary<String,Profile>> GetProfiles()
        {
            return Task.FromResult(profiles);
        }

        public Task<Profile> GetProfile(String profileid)
        {
            if (profiles.ContainsKey(profileid))
            {
                return Task.FromResult(profiles[profileid]);
            }
            else
            {
                return null;
            }
        }

        public String GetProfileName(String profileid)
        {
            if (profiles.ContainsKey(profileid))
            {
                return profiles[profileid].Name;
            }
            else
            {
                return null;
            }
        }

        public Profile  EmptyProfile()
        {
            string id = Common.RandomStringGenerator(16);

            Profile newprofile = new Profile()
            {
                Id = id,
                ChannelType = "HTTP",
                ChannelObject = new Channel(),
                Status = false

            };

            //Loading default values for the channel
            newprofile.ChannelObject.Port = 80;
            newprofile.ChannelObject.Interval = 10;
            newprofile.ChannelObject.Jitter = 10;
            newprofile.ChannelObject.HTTPRequestMethod = "GET";
            newprofile.ChannelObject.HTTPUserAgent = "Mozilla 5.0";
            newprofile.ChannelObject.HTTPURI = "/api/?id=123";

            return newprofile;
        }

        public Task<bool> AddProfile(Profile newprofile)
        {
            if (!profiles.ContainsKey(newprofile.Id))
            {
                profiles.TryAdd(newprofile.Id, newprofile);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateProfile(Profile newprofile)
        {
            if (profiles.ContainsKey(newprofile.Id))
            {
                profiles[newprofile.Id] = newprofile;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteProfile(String id)
        {

            if (profiles.ContainsKey(id))
            {
                profiles.Remove(id);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }

        }

    }
}
