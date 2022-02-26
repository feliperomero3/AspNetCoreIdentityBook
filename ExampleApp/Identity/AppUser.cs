﻿using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ExampleApp.Identity
{
    public class AppUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string EmailAddress { get; set; }

        public string NormalizedEmailAddress { get; set; }

        public bool IsEmailAddressConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsPhoneNumberConfirmed { get; set; }

        public string FavoriteFood { get; set; }

        public string Hobby { get; set; }

        public IList<Claim> Claims { get; set; }

        public string SecurityStamp { get; set; }

        public string PasswordHash { get; set; }
    }
}
