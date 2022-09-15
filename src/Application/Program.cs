using DxMLEngine.Styles;
using DxMLEngine.Operation;
using DxMLEngine.Utilities;

namespace DxMLEngine
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
                if (selection != null) Execute.ExecuteCommand(selection);
            }
            catch (Exception ex) 
            {
                Log.Error(ex.Message);
                Console.WriteLine(ex.StackTrace);

                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    Log.Error(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            Console.Write("\nPress Escape to exit...");
            var input = Console.ReadKey();
            if (input.Key != ConsoleKey.Escape) Start();
            else { Console.Clear(); return; }
        }
    }
}