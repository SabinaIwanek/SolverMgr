﻿using SQLite;

namespace AplikacjaTestujaca.Database
{
    public class Statystyki
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int IloscZadan { get; set; }
        public int IloscProcesorow { get; set; }
        public int IloscWarunkow { get; set; }
        public int CzasMax { get; set; }
        public int CzasMin { get; set; }
        public string Nazwa { get; set; }
        public string CzasAnalizyDoPorownania { get; set; }
        public string CzasAnalizy { get; set; }
    }
}