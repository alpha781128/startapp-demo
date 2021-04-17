namespace Startapp.Shared.Models
{

    public class Configuration
    {

    }


    public class UserConfiguration
    {
        public UserConfiguration()
        {
            Language = "en-US";
            Direction = "auto";
            HomeUrl = "/";
            ThemeId = 1;
            ShowDashboardStatistics = true;
            ShowDashboardNotifications = true;
            ShowDashboardTodo = true;
            ShowDashboardBanner = true;
        }
       
        public string Language { get; set; }
        public string Direction { get; set; }
        public string HomeUrl { get; set; }
        public int ThemeId { get; set; }
        public bool ShowDashboardStatistics { get; set; }
        public bool ShowDashboardNotifications { get; set; }
        public bool ShowDashboardTodo { get; set; }
        public bool ShowDashboardBanner { get; set; }
      
    }

}
