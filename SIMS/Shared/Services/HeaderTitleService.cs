using SIMS.Shared.Models;
using System.ComponentModel;

namespace SIMS.Shared.Services
{
    public class HeaderTitleService : INotifyPropertyChanged
    {
        private string _title = "Student Information Management System";

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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
