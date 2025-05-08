using AplikacjaTestujaca.Controllers;

namespace AplikacjaTestujaca
{
    public partial class App : Application
    {
        private static DBController _database;

        public static DBController Database
        {
            get
            {
                if (_database == null)
                {
                    string dbPath = Path.Combine(
                        FileSystem.AppDataDirectory, "app.db3");
                    _database = new DBController(dbPath);
                }
                return _database;
            }
        }
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}