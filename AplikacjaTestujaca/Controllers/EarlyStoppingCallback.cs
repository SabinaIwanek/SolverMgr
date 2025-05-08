using Google.OrTools.Sat;

namespace AplikacjaTestujaca.Controllers
{
    class EarlyStoppingCallback : CpSolverSolutionCallback
    {
        private readonly IntVar finishVar;
        private readonly int idealFinish;
        private readonly CpSolver solver;

        public bool FoundIdeal { get; private set; } = false;

        public EarlyStoppingCallback(IntVar finishVar, int idealFinish, CpSolver solver)
        {
            this.finishVar = finishVar;
            this.idealFinish = idealFinish;
            this.solver = solver;
        }

        public override void OnSolutionCallback()
        {
            long currentFinish = Value(finishVar);
            //Console.WriteLine($"[DEBUG] Znaleziono rozwiązanie: finish = {currentFinish}");
            if (currentFinish == idealFinish)
            {
                //Console.WriteLine($"[INFO] Osiągnięto idealny czas zakończenia: {currentFinish}. Przerywam solver.");
                FoundIdeal = true;
                StopSearch(); // ⬅️ KLUCZOWE: przerwij solver natychmiast
            }
        }
    }
}