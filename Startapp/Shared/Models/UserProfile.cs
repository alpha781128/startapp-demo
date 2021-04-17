namespace Startapp.Shared.Models
{
    public class UserProfile
    {
        public UserProfile()
        {
            FullName = "Anonymous user";
            Pic = $"/images/no-photo.jpg";
        }
        public string Pic { get; set; }
        public string FullName { get; set; }
    }
}
