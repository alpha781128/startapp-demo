using Startapp.Shared.Models;
using System.ComponentModel;

namespace Startapp.Client.Services
{
    public class UserSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserConfiguration)));
        }

        private UserConfiguration userConfiguration { get; set; } = new UserConfiguration();

        public UserConfiguration UserConfiguration
        {
            get
            {
                return this.userConfiguration;
            }
            set
            {
                if (value != this.userConfiguration)
                {
                    this.userConfiguration = value;
                    NotifyPropertyChanged();
                }
            }
        }
          

       
    }
}
