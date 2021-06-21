namespace Tehsat
{
    public class Profile 
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; }

        public string ChannelType { get; set; }

        public Channel ChannelObject { get; set; }

        private int used = 0;

        public int Used
        {
            get
            {
                return this.used;
            }
            set
            {
                this.used = value;
            }
        }

        public Profile Clone()
        {
            Profile newprofile = new Profile()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Status = this.Status,
                ChannelType = this.ChannelType,
                ChannelObject = this.ChannelObject.Clone(),
                Used = this.Used
            };

            return newprofile;
        }
    }
}
