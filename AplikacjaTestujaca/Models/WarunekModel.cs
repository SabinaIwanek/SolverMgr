using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AplikacjaTestujaca.Models
{
    public class WarunekModel : INotifyPropertyChanged
    {
        private int _poprzedzajace;
        public int Poprzedzajace
        {
            get => _poprzedzajace;
            set
            {
                if (_poprzedzajace == value) return;

                _poprzedzajace = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Poprzedzajace"));
            }
        }

        private int _nastepujace;
        public int Nastepujace
        {
            get => _nastepujace;
            set
            {
                if (_nastepujace == value) return;

                _nastepujace = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Nastepujace"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}