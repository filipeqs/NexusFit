namespace NexusFit.Auth.FunctionalTests.Helpers
{
    public static class AuthRoutes
    {
        public static class Get
        {
            public static string UserExists(string email) => $"api/auth/{email}";
        }

        public static class Post
        {
            public static string Login = "api/auth/login";
            public static string Register = "api/auth/register";
            public static string OAuthLogin = "connect/token";
        }
    }
}
