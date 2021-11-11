﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ExampleApp
{
    public static class UsersAndClaims
    {
        private static readonly Dictionary<string, IEnumerable<string>> UserData =
            new()
            {
                { "Alice", new[] { "User", "Administrator" } },
                { "Bob", new[] { "User" } },
                { "Charlie", new[] { "User" } }
            };

        public static string[] Users => UserData.Keys.ToArray();

        public static Dictionary<string, IEnumerable<Claim>> Claims =>
            UserData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(role => new Claim(ClaimTypes.Role, role)),
                StringComparer.InvariantCultureIgnoreCase);
    }
}
