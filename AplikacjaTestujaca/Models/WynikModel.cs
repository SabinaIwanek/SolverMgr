using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AplikacjaTestujaca.Models
{
    public class WynikModel : INotifyPropertyChanged
    {
        private int _p;
        private int _count;
        private bool _naglowek = false;
        public WynikModel(int time, List<string> lista, string? t)
        {
            Time = time;
            ListaZadan = lista;
            _p = 0;
            _count = lista.Count;

            if (lista[0].Contains("P")) _naglowek = true;
            T = t;
        }
        public int Time { get; set; }
        public Color BGColor => _naglowek? new Color(245, 245, 245) : Colors.White;
        public List<string> ListaZadan { get; set; }
        public string? T { get; set; }
        public string? W0 { get => _p >= 0 && _p < _count ? ListaZadan[_p].ToString() : null; }
        public string? W1 { get => _p >= 0 && _p + 1 < _count ? ListaZadan[_p + 1].ToString() : null; }
        public string? W2 { get => _p >= 0 && _p + 2 < _count ? ListaZadan[_p + 2].ToString() : null; }
        public string? W3 { get => _p >= 0 && _p + 3 < _count ? ListaZadan[_p + 3].ToString() : null; }
        public string? W4 { get => _p >= 0 && _p + 4 < _count ? ListaZadan[_p + 4].ToString() : null; }
        public string? W5 { get => _p >= 0 && _p + 5 < _count ? ListaZadan[_p + 5].ToString() : null; }
        public string? W6 { get => _p >= 0 && _p + 6 < _count ? ListaZadan[_p + 6].ToString() : null; }
        public string? W7 { get => _p >= 0 && _p + 7 < _count ? ListaZadan[_p + 7].ToString() : null; }
        public string? W8 { get => _p >= 0 && _p + 8 < _count ? ListaZadan[_p + 8].ToString() : null; }
        public string? W9 { get => _p >= 0 && _p + 9 < _count ? ListaZadan[_p + 9].ToString() : null; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void MoveP(int move)
        {
            if (move == -1 && _p == 0) return;
            _p += move;
        }
    }
}