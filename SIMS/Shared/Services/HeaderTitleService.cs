using SIMS.Shared.Models;
using System.ComponentModel;

namespace SIMS.Shared.Services
{
    public class HeaderTitleService : INotifyPropertyChanged
    {
        private string _title = "Student Information Management System";
        private string _officialAvatar = "default-avatar.png"; // Default value or path to the default avatar

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string OfficialAvatar
        {
            get => _officialAvatar;
            set
            {
                if (_officialAvatar != value)
                {
                    _officialAvatar = value;
                    OnPropertyChanged(nameof(OfficialAvatar));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
