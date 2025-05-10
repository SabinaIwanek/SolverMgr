using SQLite;

namespace AplikacjaTestujaca.Database
{
    public class Problem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Nazwa { get; set; }
        public int IloscZadan { get; set; }
        public int IloscProcesorow { get; set; }
        public int MaxCzas { get; set; }
        public string CzasDoPorownania { get; set; }
    }
}