using CutGLib;

namespace CutGLibSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CutEngine cutEngine = new CutEngine();

            cutEngine.AddLinearStock(ALength: 5000.0, aCount: 25);

            cutEngine.AddLinearPart(ALength: 150.0, aCount: 4, aAngleStart: 45.0, aAngleEnd: 45.0, aID: "Part_1");
            cutEngine.AddLinearPart(ALength: 250.0, aCount: 4, aAngleStart: 45.0, aAngleEnd: 45.0, aID: "Part_2");
            cutEngine.AddLinearPart(ALength: 100.0, aCount: 4, aID: "Part_3");

            cutEngine.SawWidth = 10.0;

            Console.WriteLine(cutEngine.ExecuteLinear());
        }
    }
}