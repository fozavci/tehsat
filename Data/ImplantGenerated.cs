using System;
using System.Collections.Generic;

namespace Tehsat
{
    public class ImplantGenerated
    {
        // Generic implant options
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Dictionary<String, Channel> Channels { get; set; }

        // Cloning
        public ImplantGenerated Clone()
        {
            ImplantGenerated newimplantgenerated = new ImplantGenerated()
            {
                Id = this.Id,
                Name = this.Name,
                Status = this.Status,
                Description = this.Description,
                Channels = this.Channels,
            };

            return newimplantgenerated;
        }
        
    }
}
