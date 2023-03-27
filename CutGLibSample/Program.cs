using CutGLib;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CutGLibSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CutEngine cutEngine = new CutEngine();

            cutEngine.AddLinearStock(ALength: 1000.0, aCount: 25);

            cutEngine.AddLinearPart(ALength: 150.0, aCount: 4, aAngleStart: 45.0, aAngleEnd: 45.0, aID: "Part_1");
            cutEngine.AddLinearPart(ALength: 250.0, aCount: 4, aAngleStart: 45.0, aAngleEnd: 45.0, aID: "Part_2");
            cutEngine.AddLinearPart(ALength: 100.0, aCount: 4, aID: "Part_3");

            var resultMessage = cutEngine.ExecuteLinear();

            var linearCutsCount = cutEngine.GetLinearCutsCount();
            var linearCuts = new List<object>();
            var maxStockCutCount = 0;
            for (int i = 0; i < linearCutsCount; i++)
            {
                cutEngine.GetLinearCut(i, out int stock, out double location);
                linearCuts.Add(new { Index = i, Stock = stock, Location = location });
                maxStockCutCount = stock;
            }

            var linearStockCuts = new List<object>();
            for (int i = 0; i < maxStockCutCount; i++)
            {
                var stockCutCount = cutEngine.GetStockCutCount(i);

                for (int j = 0; j < stockCutCount; j++)
                {
                    cutEngine.GetLinearStockCut(i, j, out double location, out double angle);
                    linearStockCuts.Add(new { Stock = i, Cut = j, Location = location, Angle = angle });
                }
            }

            var partCount = cutEngine.PartCount;
            var resultLinearParts = new List<object>();

            for (int i = 0; i <= partCount; i++)
            {
                var fromStock = cutEngine.GetResultLinearPart(i, out int stock, out double length, out double angleStart, out double angleEnd, out double location, out bool rotated, out bool flipped, out string id);
                resultLinearParts.Add(new { Part = i, Stock = stock, Length = length, AngleStart = angleStart, AngleEnd = angleEnd, Rotated = rotated, Flipped = flipped, Location = location, Id = id, FromStock = fromStock });
            }
            

            object cutResult = new
            {
                ResultMessage = resultMessage,
                LinearCutsCount = cutEngine.GetLinearCutsCount(),
                LinearCuts = linearCuts,
                MaxStockCutCount = maxStockCutCount,
                LinearStockCuts = linearStockCuts,
                PartCount = partCount,
                ResultLinearParts = resultLinearParts,
            };

            Console.WriteLine(JsonConvert.SerializeObject(cutResult, Formatting.Indented));
        }
    }
}