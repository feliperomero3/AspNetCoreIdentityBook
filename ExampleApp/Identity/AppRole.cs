using System;

namespace ExampleApp.Identity
{
    public class AppRole
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }
}
