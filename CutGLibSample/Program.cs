using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CutGLibSample
{
    internal class Program
    {
        private static CuttingOptimization.CuttingOptimizationInputStock ironRod6Meters = new CuttingOptimization.CuttingOptimizationInputStock(id: "Iron Rod - 6m", length: 6000.0, count: 5, isWaste: false);
        private static CuttingOptimization.CuttingOptimizationInputStock ironRod4Meters = new CuttingOptimization.CuttingOptimizationInputStock(id: "Iron Rod - 4m", length: 4000.0, count: 5, isWaste: false);

        static void Main(string[] args)
        {
            Console.WriteLine("=== Single Result");
            Console.WriteLine(JsonConvert.SerializeObject(TestSinglePerformance(), Formatting.Indented));

            Console.WriteLine("=== Single");
            Console.WriteLine(TestSinglePerformance().ElapsedTime);

            Console.WriteLine("=== Mutiple");
            int threads = 25;
            int threadCounter = 0;
            TestMultiplePerformance(threads, (result) =>
            {
                Console.WriteLine($"{result.ElapsedTime}");
                Interlocked.Increment(ref threadCounter);
            });
            while (threadCounter < threads) {  }

            Console.WriteLine("=== Angles");
            var cutOptiResult = AngleTest().GetResult();
            Console.WriteLine();
        }

        public static CuttingOptimization GetCuttingOptimization()
        {
            var cutOpti = new CuttingOptimization
            {
                SawBladeWidth = 6.0,
                BufferLeft = 15.0,
                BufferRight = 0.0,
                UseLinearSortAscending = true,
                UseLinearExactAngle = false,
                AllowLinearFlipping = true,
                AllowLinearRotation = true,
                UseLargeStockFirst = true,
                UseCompleteMode = true
            };
            cutOpti.AddLinearStock(ironRod6Meters);
            cutOpti.AddLinearStock(ironRod4Meters);
            cutOpti.AddLinearPart(new CuttingOptimization.CuttingOptimizationInputLinearPart(id: "Iron Rod - 1.25m", count: 4, length: 1250.0, angleStart: 90.0, angleEnd: 90.0));
            cutOpti.AddLinearPart(new CuttingOptimization.CuttingOptimizationInputLinearPart(id: "Iron Rod - 0.77m, AS 45°, AE 45°", count: 8, length: 770.0, angleStart: 45.0, angleEnd: 45.0));
            cutOpti.AddLinearPart(new CuttingOptimization.CuttingOptimizationInputLinearPart(id: "Iron Rod - 0.77m, AS 45°, AE 90°", count: 10, length: 77.0, angleStart: 45.0, angleEnd: 90.0));
            cutOpti.AddLinearPart(new CuttingOptimization.CuttingOptimizationInputLinearPart(id: "Iron Rod - 0.90m, AS 63°, AE 43°", count: 10, length: 900.0, angleStart: 63.0, angleEnd: 43.0));
            return cutOpti;
        }

        public static CuttingOptimization.CuttingOptimizationResult TestSinglePerformance()
        {
            return GetCuttingOptimization().GetResult();
        }

        public static void TestMultiplePerformance(int threads, Action<CuttingOptimization.CuttingOptimizationResult> onResult)
        {
            var tasks = new List<Task>();
            for (int i = 1; i <= threads; i++)
            {
                tasks.Add(new Task(() =>
                {
                    var cutOpti = GetCuttingOptimization();
                    onResult.Invoke(cutOpti.GetResult());
                }));
            }
            tasks.ForEach(x =>
            {
                x.Start();
            });
        }

        public static CuttingOptimization AngleTest()
        {
            var cutOpti = new CuttingOptimization
            {
                SawBladeWidth = 6.0,
                BufferLeft = 15.0,
                BufferRight = 0.0,
                UseLinearSortAscending = true,
                UseLinearExactAngle = false,
                AllowLinearFlipping = true,
                AllowLinearRotation = true,
                UseLargeStockFirst = true,
                UseCompleteMode = true
            };
            cutOpti.AddLinearStock(ironRod6Meters);
            cutOpti.AddLinearStock(ironRod4Meters);
            cutOpti.AddLinearPart(new CuttingOptimization.CuttingOptimizationInputLinearPart(id: "Iron Rod - 1m", count: 4, length: 1000.0, angleStart: 90.0, angleEnd: 45.0));
            return cutOpti;
        }

    }
}