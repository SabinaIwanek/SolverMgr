using AplikacjaTestujaca.Controllers;
using AplikacjaTestujaca.Database;
using AplikacjaTestujaca.Models;
using System.Collections.ObjectModel;
using System.Text;
using System.Timers;

namespace AplikacjaTestujaca
{
    public partial class MainPage : ContentPage
    {
        #region Props
        private bool first = true;
        private bool delete = false;

        private List<string> _listProblemsItems = new List<string>
      {
          "Przykład 1",
      };

        private System.Timers.Timer taskTimer;
        private int secondsElapsed = 0;
        public static DBController Database { get; private set; }

        ObservableCollection<WarunekModel> _warunkiList;
        ObservableCollection<Statystyki> _statystykiList;
        ObservableCollection<WynikModel> _wynik;

        ProblemModel _model;
        public ProblemModel Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            Database = new DBController(dbPath);

            Model = new ProblemModel();
            _warunkiList = new ObservableCollection<WarunekModel>();

            //Obsługa pickera
            CountryPicker.SelectedIndexChanged += CountryPicker_SelectedIndexChanged;

            //Obsługa timera
            taskTimer = new System.Timers.Timer(1000); // co 1 sekunda
            taskTimer.Elapsed += OnTimerElapsed;
        }

        #region Menu górne
        public void OnFormClicked(object sender, EventArgs e)
        {
            //Dane
            DaneBaza.IsVisible = true;
            Dane.IsVisible = true;
            Tabela.IsVisible = _wynik != null;

            //Statystyki
            StatystykiNaglowek.IsVisible = false;
            collectionViewStatystyki.IsVisible = false;
        }
        public void OnStateClicked(object sender, EventArgs e)
        {
            //Dane
            DaneBaza.IsVisible = false;
            Dane.IsVisible = false;
            Tabela.IsVisible = false;

            //Statystyki
            StatystykiNaglowek.IsVisible = true;
            collectionViewStatystyki.IsVisible = true;
        }
        #endregion

        #region Picker
        async private void CountryPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = CountryPicker.SelectedItem as string;
            if (selected == null) return;

            var problems = await Database.GetProblemsAsync();
            var model = problems.First(x => x.Nazwa == selected);

            Model = new ProblemModel
            {
                Nazwa = model.Nazwa,
                IloscZadan = model.IloscZadan,
                IloscProcesorow = model.IloscProcesorow,
                CzasMax = model.MaxCzas,
            };

            var warunki = (await Database.GetWarunkiAsync())
                    .Where(w => w.ID_Problem == model.ID)
                    .ToList();

            _warunkiList = new ObservableCollection<WarunekModel>(warunki.Select(x => new WarunekModel() { Poprzedzajace = x.Poprzedzajace, Nastepujace = x.Nastepujace }).ToList());
            OnAppearing();
        }
        #endregion

        #region Timer
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            secondsElapsed++;
            // Aktualizacja UI musi być wykonana na głównym wątku
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerLabel.Text = $"Czas: {secondsElapsed} s";
            });
        }
        #endregion

        #region Lista

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_warunkiList.Any() && !delete)
            {
                delete = false;

                var problems = await Database.GetProblemsAsync();
                var dane = problems.FirstOrDefault(x => x.Nazwa == Model.Nazwa);

                if (dane != null)
                {
                    var warunki = await Database.GetWarunkiAsync();
                    var filtered = warunki.Where(x => x.ID_Problem == dane.ID).ToList();
                    var lista = filtered.Select(x => new WarunekModel
                    {
                        Poprzedzajace = x.Poprzedzajace,
                        Nastepujace = x.Nastepujace
                    }).ToList();

                    _warunkiList = new ObservableCollection<WarunekModel>(lista);
                }
            }

            collectionView.ItemsSource = _warunkiList;

            if (first)
            {
                first = false;
                ListPicker();
                ListState();
            }
        }

        async void ListPicker()
        {
            CountryPicker.ItemsSource = null;
            _listProblemsItems = new List<string>();

            var problemsList = await Database.GetProblemsAsync();
            var nazwy = problemsList.Select(p => p.Nazwa).ToArray();

            foreach (var item in nazwy)
            {
                _listProblemsItems.Add(item);
            }

            CountryPicker.ItemsSource = _listProblemsItems;
        }
        async void ListState()
        {
            collectionViewStatystyki.ItemsSource = null;

            _statystykiList = new ObservableCollection<Statystyki>(await Database.GetStatystykiAsync());

            collectionViewStatystyki.ItemsSource = _statystykiList;
        }
        #endregion

        #region Buttons
        async private void OnAddButtonClicked(object sender, EventArgs e)
        {
            Problem model = new Problem()
            {
                Nazwa = EntryNazwa.Text,
                IloscZadan = Model.IloscZadan,
                IloscProcesorow = Model.IloscProcesorow,
                MaxCzas = Model.CzasMax,
            };

            await Database.AddProblemAsync(model);

            foreach (var item in _warunkiList)
            {
                Warunek warunek = new Warunek
                {
                    ID_Problem = model.ID,
                    Poprzedzajace = item.Poprzedzajace,
                    Nastepujace = item.Nastepujace,
                };

                Database.AddWarunekAsync(warunek);
            }

            ListPicker();
        }
        async private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var problems = await Database.GetProblemsAsync();
            var id = problems.First(x => x.Nazwa == Model.Nazwa).ID;

            Problem model = new Problem()
            {
                ID = id,
                Nazwa = Model.Nazwa,
                IloscZadan = Model.IloscZadan,
                IloscProcesorow = Model.IloscProcesorow,
                MaxCzas = Model.CzasMax,
            };

            await Database.UpdateProblemAsync(model);

            var poprzednie = (await Database.GetWarunkiAsync())
                    .Where(w => w.ID_Problem == model.ID)
                    .ToList();

            foreach (var item in poprzednie)
            {
                Database.DeleteWarunekAsync(item);
            }

            foreach (var item in _warunkiList)
            {
                Warunek warunek = new Warunek
                {
                    ID_Problem = model.ID,
                    Poprzedzajace = item.Poprzedzajace,
                    Nastepujace = item.Nastepujace,
                };

                Database.AddWarunekAsync(warunek);
            }
        }
        async private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var problems = await Database.GetProblemsAsync();
            var id = problems.First(x => x.Nazwa == Model.Nazwa).ID;

            Problem model = new Problem()
            {
                ID = id,
                Nazwa = Model.Nazwa,
                IloscZadan = Model.IloscZadan,
                IloscProcesorow = Model.IloscProcesorow,
                MaxCzas = Model.CzasMax,
            };

            Database.DeleteProblemAsync(model);

            var poprzednie = (await Database.GetWarunkiAsync())
                    .Where(w => w.ID_Problem == model.ID)
                    .ToList();

            foreach (var item in poprzednie)
            {
                Database.DeleteWarunekAsync(item);
            }

            ListPicker();
        }
        private void OnAddElemButtonClicked(object sender, EventArgs e)
        {
            _warunkiList.Add(new WarunekModel() { Poprzedzajace = Model.Poprzedzajace, Nastepujace = Model.Nastepujace });
            OnAppearing();
        }
        private void OnDeleteElemButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (WarunekModel)button.Parent.BindingContext;

            _warunkiList.Remove(item);
            delete = true;
            OnAppearing();
        }
        async private void OnImportButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var customTextType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "text/plain" } },
                    { DevicePlatform.iOS, new[] { "public.plain-text" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.plain-text" } },
                    { DevicePlatform.WinUI, new[] { ".txt" } }
                });

                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Wybierz plik tekstowy",
                    FileTypes = customTextType
                });

                if (fileResult != null)
                {
                    using var stream = await fileResult.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    string content = await reader.ReadToEndAsync();

                    // Tutaj możesz zrobić coś z wczytanymi danymi
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"{ex.Message}", "OK");
            }
        }
        async private void OnSolveButtonClicked(object sender, EventArgs e)
        {
            #region Sprawdzanie poprawności danych

            StringBuilder sb = new StringBuilder("");
            if (Model.IloscZadan < 1) sb.AppendLine($"Ilość zadań musi być większa od 0.");
            if (Model.IloscProcesorow < 1) sb.AppendLine($"Ilość procesorów musi być większa od 0.");
            if (Model.CzasMax < 1) sb.AppendLine($"Czas maksymalny musi być większy od 0.");

            if(sb.ToString() != string.Empty)
            {
                await DisplayAlert("Błędne dane wejściowe.", $"{sb}", "OK");
                return;
            }
            #endregion

            bool error = false;
            LoadingIndicator.IsVisible = true;
            SolveButton.IsVisible = false;

            secondsElapsed = 0;
            TimerLabel.Text = $"Czas: {secondsElapsed} s";
            taskTimer.Start();

            try
            {
                var wynik = await Task.Run(() =>
                SolverController.Solve(
                    Model.IloscZadan,
                    Model.IloscProcesorow,
                    Model.CzasMax,
                    _warunkiList.Select(x => (x.Poprzedzajace, x.Nastepujace)).ToList()
                ));

                _wynik = new ObservableCollection<WynikModel>(wynik);

                collectionViewWyniki.ItemsSource = _wynik;
            }
            catch (Exception ex)
            {
                error = true;
                await DisplayAlert("Błąd", $"{ex.Message}", "OK");
            }

            taskTimer.Stop();

            if (!error)
            {
                Statystyki statystyki = new Statystyki()
                {
                    IloscZadan = Model.IloscZadan,
                    IloscProcesorow = Model.IloscProcesorow,
                    IloscWarunkow = _warunkiList.Count(),
                    CzasMax = Model.CzasMax,
                    CzasMin = _wynik.Count() - 1,
                    CzasAnalizy = secondsElapsed
                };

                Database.AddStatystykiAsync(statystyki);
            }

            LoadingIndicator.IsVisible = false;
            SolveButton.IsVisible = true;

            Tabela.IsVisible = true;
            collectionViewWyniki.IsVisible = true;

            ListState();
        }
        private void OnLeftButtonClicked(object sender, EventArgs e)
        {
            foreach (var item in _wynik)
            {
                item.MoveP(-1);
            }

            collectionViewWyniki.ItemsSource = null;
            collectionViewWyniki.ItemsSource = _wynik;
        }
        private void OnRightButtonClicked(object sender, EventArgs e)
        {
            foreach (var item in _wynik)
            {
                item.MoveP(1);
            }

            collectionViewWyniki.ItemsSource = null;
            collectionViewWyniki.ItemsSource = _wynik;
        }
        #endregion
    }
}