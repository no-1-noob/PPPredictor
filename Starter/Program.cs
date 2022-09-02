using System;

namespace Starter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TestPPPredictor();
        }

        private static void TestPPPredictor()
        {
            PPPredictor.Utilities.PPCalculatorBeatLeader test = new PPPredictor.Utilities.PPCalculatorBeatLeader();
            Console.WriteLine(test.CalculatePPatPercentage(10.65, 97.23485));
        }
    }
}
