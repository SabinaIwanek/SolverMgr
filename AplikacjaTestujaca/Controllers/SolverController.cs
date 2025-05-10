using AplikacjaTestujaca.Models;
using Google.OrTools.LinearSolver;

namespace AplikacjaTestujaca.Controllers
{
    public static class SolverController
    {
        public static List<WynikModel> Solve(int numTasks, int m, int D, List<(int, int)> precedence)
        {
            List<WynikModel> wynik = new List<WynikModel>();

            int duration = 1;  // czas trwania zadania

            #region Definicja binarnych zmiennych decyzyjnych
            Solver model = Solver.CreateSolver("SCIP");

            // Deklaracja tablicy 3 wymiarowej, ponieważ harmonogram zawiera: zadania, procesory i czas startu
            Variable[,,] x = new Variable[numTasks, m, D];

            for (int t = 0; t < numTasks; t++)
            {
                for (int p = 0; p < m; p++)
                {
                    for (int time = 0; time < D; time++)
                    {
                        // Zmienna x[t, p, time] = 1 jeśli zadanie t wykonywane na procesorze p w czasie time, jeśli nie to równa zero
                        x[t, p, time] = model.MakeBoolVar($"x_{t}_{p}_{time}"); //przypisujemy każdej unikalną nazwę
                    }
                }
            }
            #endregion

            #region Ograniczenia podstawowe
            // Ograniczenie: Każde zadanie musi być wykonane dokładnie raz
            for (int t = 0; t < numTasks; t++)
            {
                List<Variable> occurences = new();
                for (int p = 0; p < m; p++)
                    for (int time = 0; time < D; time++)
                        occurences.Add(x[t, p, time]);

                //occurences(zdarzenia) => wszystkie zmienne dot 1 zadania, poniżej warunek, że suma tej listy musi być =1
                //model.Add(LinearExpr.Sum(occurences) == 1);

                //[DODANE]
                LinearExpr sumExpr = occurences[0];
                for (int s = 1; s < occurences.Count; s++)
                {
                    sumExpr += occurences[s];
                }
                model.Add(sumExpr == 1);
            }

            // Ograniczenie: Każdy procesor może robić jedno zadanie na raz
            for (int p = 0; p < m; p++)
            {
                for (int time = 0; time < D; time++)
                {
                    List<Variable> simultaneous = new();
                    for (int t = 0; t < numTasks; t++)
                        simultaneous.Add(x[t, p, time]);

                    //simultaneous(jednoczesny) => wszystkie zmienne o tym zamym procesorze i czasie, poniżej warunek że suma nie może przekraczać 1
                    //czyli albo procesor w danym czasie jest zajęty albo wolny
                    //model.Add(LinearExpr.Sum(simultaneous) <= 1);

                    //[DODANE]
                    LinearExpr sumExpr = simultaneous[0];
                    for (int s = 1; s < simultaneous.Count; s++)
                    {
                        sumExpr += simultaneous[s];
                    }
                    model.Add(sumExpr <= 1);
                }
            }
            #endregion

            #region Ograniczenia dotyczące czasów

            // wpisanie do modelu wyznaczenia czasu startu dla każdego zadania
            Variable[] startTime = new Variable[numTasks];
            for (int t = 0; t < numTasks; t++)
            {
                //z funkcji otrzymujemy IntVar, która trzyma dokładny moment rozpoczęcia zadania.
                startTime[t] = WeightedSum(model, x, t, m, D);
            }

            //Dodanie realizacji częściowego porządku
            foreach ((int before, int after) in precedence)
            {
                //jest >= dlatego dodajemy duration, w tym przypadku można usunąć duration i dać tylko >
                model.Add(startTime[after] >= startTime[before] + duration);
            }

            // Ograniczenie maksymalnego czasu
            foreach (var s in startTime)
            {
                model.Add(s <= D - 1);
            }
            #endregion

            #region Minimalizacja w celu otrzymania najszybszej opcji
            // Zmienna celu - maksymalny czas zakończenia
            Variable finishTime = model.MakeIntVar(0, D, "finish");

            foreach (var s in startTime)
            {
                //definiujemy finishTime jako największy z czasów zakończenia wszystkich zadań
                model.Add(finishTime >= s + duration);
            }

            //przekazujemy informacje do modelu aby znalazł opcję z najmniejszym finishTime
            model.Minimize(finishTime);
            #endregion

            #region wywołanie solvera i wypisanie wyniku
            // Solver

            Solver.ResultStatus status = model.Solve();

            List<(int z, int t)> lista = new List<(int z, int t)>();

            //Feasible jeśli próba znalezienia trwa za długo to daje poprawne rozwiązanie ale nie wie czy jest ono najlepsze
            if (status == Solver.ResultStatus.OPTIMAL || status == Solver.ResultStatus.FEASIBLE)
            {
                //Console.WriteLine($"Rozwiązanie: finish = {solver.Value(finishTime)}");
                for (int t = 0; t < numTasks; t++)
                {
                    //wyciągamy z solwera wartości zmiennych zadeklarowanych do modelu
                    //Console.WriteLine($"Zadanie {t} startuje w czasie {solver.Value(startTime[t])}");
                    lista.Add((t, (int)startTime[t].SolutionValue()));
                }
            }
            else
            {
                throw new Exception("Dla podanych danych nie ma rozwiązania.");
            }
            #endregion

            var grupy = lista.GroupBy(x => x.t).OrderBy(x => x.Key);

            wynik.Add(new WynikModel(-1, new List<string> { "P1", "P2", "P3", "P4", "P5", "P6", "P7", "P8", "P9", "P10", "P11", "P12", "P13", "P14", "P15", "P16", "P17", "P18", "P19", "P20" }.ToList(), null));

            int i = 0;

            foreach (var item in grupy)
            {
                //dodaje 1 bo solver działa od 0, ale wyniki prezentuje od 1
                wynik.Add(new WynikModel(item.Key, item.Select(x => (x.z + 1).ToString()).ToList(), $"T{++i}"));
            }

            return wynik;
        }

        // Funkcja pomocnicza, do wyznaczania czasu startu na podstawie binarnych zmiennych x[t,p,t]
        static Variable WeightedSum(Solver model, Variable[,,] x, int task, int numProc, int horizon)
        {
            List<Variable> vars = new(); // lista zmiennych decyzyjnych dla danego task
            List<int> coeffs = new(); // współczynniki czasowe (t) odpowiadające tym zmiennym

            for (int p = 0; p < numProc; p++)
            {
                for (int t = 0; t < horizon; t++)
                {
                    vars.Add(x[task, p, t]);
                    coeffs.Add(t);
                }
            }

            //[DODANE] // Budowanie wyrażenia: sum(t * x[task,p,t])
            LinearExpr sumExpr = vars[0] * coeffs[0];
            for (int i = 1; i < vars.Count; i++)
            {
                sumExpr += vars[i] * coeffs[i];
            }

            //do modelu dodajemy zmienną przechowującą czas startowy zadania
            Variable start = model.MakeIntVar(0, horizon - 1, $"start_{task}");

            //korzystamy z skalarnego iloczynu dzięki któremu otrzymamy czas startowy 
            //czyli tylko jedna zmienna w vars będzie równa 1, reszta 0, 
            //czyli tam gdzie vars = 1, to my otrzymamy przypisany do niego coeffs
            //model.Add(start == LinearExpr.ScalProd(vars, coeffs));

            model.Add(start == sumExpr);

            return start;
        }
    }
}