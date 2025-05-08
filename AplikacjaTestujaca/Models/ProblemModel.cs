using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AplikacjaTestujaca.Models
{
    public class ProblemModel : INotifyPropertyChanged
    {
        private string _nazwa;
        public string Nazwa
        {
            get => _nazwa;
            set
            {
                if (_nazwa == value) return;

                _nazwa = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Nazwa"));
            }
        }

        private int _iloscZadan;
        public int IloscZadan
        {
            get => _iloscZadan;
            set
            {
                if (_iloscZadan == value) return;

                _iloscZadan = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IloscZadan"));
            }
        }
        
        private int _iloscProcesorow;
        public int IloscProcesorow
        {
            get => _iloscProcesorow;
            set
            {
                if (_iloscProcesorow == value) return;

                _iloscProcesorow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IloscProcesorow"));
            }
        }
        
        private int _czasMax;
        public int CzasMax
        {
            get => _czasMax;
            set
            {
                if (_czasMax == value) return;

                _czasMax = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CzasMax"));
            }
        }

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
