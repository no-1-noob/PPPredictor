using System;

namespace Starter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var date = new DateTime(1970, 1, 1).AddSeconds(1661870422);
            Console.WriteLine(date);
            //TestPPPredictor();
        }

        private static void TestPPPredictor()
        {
            /*PPPredictor.Utilities.PPCalculatorBeatLeader test = new PPPredictor.Utilities.PPCalculatorBeatLeader(null);
            Console.WriteLine(test.CalculatePPatPercentage(14.4205002858751, 97.68));
            Console.WriteLine(test.CalculatePPatPercentage(14.4205002858751, 90));
            Console.WriteLine(test.CalculatePPatPercentage(4.4205002858751, 97.68));
            Console.WriteLine(test.CalculatePPatPercentage(4.4205002858751, 90));
            Console.WriteLine(test.CalculatePPatPercentage(14.80, 12.61));*/
        }
    }
}
