namespace UniProject.Models
{
    //This class to hold currnet user info
    public static class GlobalVars
    {
        public static string currentUser { get; set; }
        public static bool isLoggedIn { get; set; }
        public static string currentUserEmail { get; set; }

        public static bool isCurrentUserAdmin { get; set; }
    }
}
