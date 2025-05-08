using AplikacjaTestujaca.Database;
using SQLite;

namespace AplikacjaTestujaca.Controllers
{
    public class DBController
    {
        private readonly SQLiteAsyncConnection _database;

        public DBController(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Problem>().Wait();
            _database.CreateTableAsync<Warunek>().Wait();
            _database.CreateTableAsync<Statystyki>().Wait();
        }

        // CRUD dla Problem
        public Task<List<Problem>> GetProblemsAsync() =>
            _database.Table<Problem>().ToListAsync();

        public Task<int> AddProblemAsync(Problem item) =>
            _database.InsertAsync(item);

        public Task<int> UpdateProblemAsync(Problem item) =>
            _database.UpdateAsync(item);

        public Task<int> DeleteProblemAsync(Problem item) =>
            _database.DeleteAsync(item);

        // CRUD dla Warunek
        public Task<List<Warunek>> GetWarunkiAsync() =>
            _database.Table<Warunek>().ToListAsync();

        public Task<int> AddWarunekAsync(Warunek item) =>
            _database.InsertAsync(item);

        public Task<int> DeleteWarunekAsync(Warunek item) =>
            _database.DeleteAsync(item);

        // CRUD dla Statystyki
        public Task<List<Statystyki>> GetStatystykiAsync() =>
            _database.Table<Statystyki>().ToListAsync();

        public Task<int> AddStatystykiAsync(Statystyki item) =>
            _database.InsertAsync(item);

        public Task<int> DeleteStatystykiAsync(Statystyki item) =>
            _database.DeleteAsync(item);
    }
}
