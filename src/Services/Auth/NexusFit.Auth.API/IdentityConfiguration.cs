using IdentityServer4;
using IdentityServer4.Models;

namespace NexusFit.Auth.API
{
    public class IdentityConfiguration
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResource("roles", "Your role(s)", new List<string> { "role" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("NexusFit.Exercises.API"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("ExercisesAPI")
                {
                    Scopes = new List<string>{ "NexusFit.Exercises.API" },
                    UserClaims = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.Email,
                        "role"
                    }
                },
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "NexusFit.API.Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "NexusFit.Exercises.API" }
                }
            };
    }
}
