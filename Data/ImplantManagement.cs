using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tehsat
{
    public class ImplantManagement
    {
        public static Dictionary<String, ImplantGenerated> implantsgenerated = new Dictionary<String, ImplantGenerated>();
        
        public Task<Dictionary<String, ImplantGenerated>> GetImplantsGenerated()
        {
            return Task.FromResult(implantsgenerated);
        }

        public Task<ImplantGenerated> GetImplantGenerated(String igid)
        {
            if (implantsgenerated.ContainsKey(igid))
            {
                return Task.FromResult(implantsgenerated[igid]);
            }
            else
            {
                return null;
            }
        }

        public Task<String> GetImplantGeneratedName(String igid)
        {
            if (implantsgenerated.ContainsKey(igid))
            {
                return Task.FromResult(implantsgenerated[igid].Name);
            }
            else
            {
                return null;
            }
        }

        
        public Task<bool> AddImplantGenerated(ImplantGenerated nig)
        {
            if (!implantsgenerated.ContainsKey(nig.Id))
            {
                implantsgenerated.TryAdd(nig.Id, nig);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> UpdateImplantGenerated(ImplantGenerated nig)
        {
            if (implantsgenerated.ContainsKey(nig.Id))
            {
                implantsgenerated[nig.Id] = nig;
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> DeleteImplantGenerated(String igid)
        {

            if (implantsgenerated.ContainsKey(igid))
            {
                implantsgenerated.Remove(igid);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }

        }

        public ImplantGenerated EmptyImplantGenerated()
        {
            string id = Common.RandomStringGenerator(16);

            ImplantGenerated newig = new ImplantGenerated()
            {
                Id = id,
                Channels = new Dictionary<String, Channel>(),
            };

            return newig;
        }

    }
}
