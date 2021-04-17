using Startapp.Shared.Models;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Startapp.Client.Services
{
    public class CurrentUserInfo : INotifyPropertyChanged
    {
        private readonly IClientUserService _user;

        public event PropertyChangedEventHandler PropertyChanged;

        public UserDTO CurrentUser { get; set; } = new UserDTO();
        public int Counter { get; set; } = 0;
        private void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUser)));
        }

        public CurrentUserInfo(IClientUserService user)
        {
            _user = user;
            _user.UserAuthenticatedEvent += _user_UserAuthenticatedEvent;
        }

        private async void _user_UserAuthenticatedEvent(object sender, UserAuthenticatedArgs e)
        {
            if (!string.IsNullOrEmpty(e.UserId))
            {
                await PopulateUser(e.UserId);
            }
            else
            {
                ClearUser();
            }
        }

        public async Task PopulateUser(string id)
        {
            CurrentUser = await _user.GetUserInfo(id);
            NotifyPropertyChanged();
        }
        public void ClearUser()
        {
            CurrentUser = new UserDTO();
            Counter = 0;
            NotifyPropertyChanged();
        }
    }
}
