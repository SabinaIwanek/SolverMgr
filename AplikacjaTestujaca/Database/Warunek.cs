using SQLite;

namespace AplikacjaTestujaca.Database
{
    public class Warunek
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int ID_Problem { get; set; }
        public int Poprzedzajace { get; set; }
        public int Nastepujace { get; set; }
    }
}