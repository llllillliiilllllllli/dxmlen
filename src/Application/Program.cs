using MLEngine.Styles;
using MLEngine.Operation;

namespace MLEngine
{
    public class Program
    {
        public static void Main()
        {
            var beg = DateTime.Now;

            Start();

            var end = DateTime.Now;
            var duration = end - beg;
            Console.WriteLine($"\nProgram finished in {duration.TotalSeconds:F2} sec");
            Environment.Exit(0);
        }

        public static void Start()
        {            
            UI.PrintConsole();

            try
            {
                var features = Reflect.CollectFeatures();
                var selection = Select.InquireSelection(features);
                var execution = Execute.ExecuteCommand(selection!);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }

            Console.Write("\nPress Escape to exit...");
            var input = Console.ReadKey();
            if (input.Key != ConsoleKey.Escape) Start();
            else { Console.Clear(); return; }
        }
    }
}