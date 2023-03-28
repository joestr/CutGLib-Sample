using CutGLib;

namespace CutGLibSample
{
    public class CuttingOptimization
    {
        private CutEngine CutEngine { get; set; } = new CutEngine();

        public double SawBladeWidth { get => CutEngine.SawWidth; set => CutEngine.SawWidth = value; }
        public double BufferLeft { get => CutEngine.TrimLeft; set => CutEngine.TrimLeft = value; }
        public double BufferRight { get => CutEngine.TrimRight; set => CutEngine.TrimRight = value; }
        public double BufferTop { get => CutEngine.TrimTop; set => CutEngine.TrimTop = value; }
        public double BufferBottom { get => CutEngine.TrimBottom; set => CutEngine.TrimBottom = value; }
        public bool UseLayoutMinimization { get => CutEngine.UseLayoutMinimization; set => CutEngine.UseLayoutMinimization = value; }
        public int MaxLayoutSize { get => CutEngine.MaxLayoutSize; set => CutEngine.MaxLayoutSize = value; }
        public double WasteSizeMin { get => CutEngine.WasteSizeMin; set => CutEngine.WasteSizeMin = value; }
        public string Version { get => CutEngine.Version; }
        public bool UseCompleteMode { get => CutEngine.CompleteMode; set => CutEngine.CompleteMode = value; }
        public bool MinimizeSheetRotation { get => CutEngine.MinimizeSheetRotation; set => CutEngine.MinimizeSheetRotation = value; }
        public int MaxCutLevel { get => CutEngine.MaxCutLevel; set => CutEngine.MaxCutLevel = value; }
        public bool UseLinearSortAscending { get => CutEngine.LinearSortAscending; set => CutEngine.LinearSortAscending = value; }
        public bool UseLinearExactAngle { get => CutEngine.LinearExactAngle; set => CutEngine.LinearExactAngle = value; }
        public bool AllowLinearRotation { get => CutEngine.LinearAllowRotate; set => CutEngine.LinearAllowRotate = value; }
        public bool AllowLinearFlipping { get => CutEngine.LinearAllowFlipping; set => CutEngine.LinearAllowFlipping = value; }
        public bool UseLargeStockFirst { get => CutEngine.UseLargeStockFirst; set => CutEngine.UseLargeStockFirst = value; }

        public CuttingOptimization()
        {
        }

        public void AddLinearStock(params CuttingOptimizationInputStock[] cuttingOptimizationInputStocks)
        {
            foreach (var stock in cuttingOptimizationInputStocks)
            {
                CutEngine.AddLinearStock(stock.Length, stock.Count, stock.Id, stock.IsWaste);
            }
        }

        public void AddLinearPart(params CuttingOptimizationInputLinearPart[] cuttingOptimizationInputLinearParts)
        {
            foreach(var part in cuttingOptimizationInputLinearParts)
            {
                CutEngine.AddLinearPart(part.Length, part.Count, part.AngleStart, part.AngleEnd, part.Id);
            }
        }

        public CuttingOptimizationResult GetResult()
        {
            return ResolveCutEngineResult(CutEngine);
        }

        private CuttingOptimizationResult ResolveCutEngineResult(CutEngine cutEngine, string? message = null)
        {
            var result = new CuttingOptimizationResult();

            if (message == null)
            {
                result.Message = cutEngine.ExecuteLinear();
            }

            result.ElapsedTime = cutEngine.ElapsedTime;

            for (int layoutIndex = 0; layoutIndex < cutEngine.LayoutCount; layoutIndex++)
            {
                cutEngine.GetLayoutInfo(layoutIndex, out int layoutStockIndex, out int layoutStockCount);
                result.Layouts.Add(ResolveCutEngineLayout(cutEngine, layoutIndex, layoutStockIndex, layoutStockCount));
            }

            return result;
        }

        private CuttingOptimizationResultLayout ResolveCutEngineLayout(CutEngine cutEngine, int layoutIndex, int layoutStockIndex, int loutStockCount)
        {
            var result = new CuttingOptimizationResultLayout() { Index = layoutIndex };

            for (int stockIndex = layoutStockIndex; stockIndex < layoutStockIndex + loutStockCount; stockIndex++)
            {
                cutEngine.GetLinearStockInfo(stockIndex, out double stockLength, out bool stockActive, out string stockId);
                result.Stocks.Add(ResolveCutEngineStock(cutEngine, stockIndex, stockLength, stockActive, stockId));
            }

            return result;
        }

        private CuttingOptimizationResultStock ResolveCutEngineStock(CutEngine cutEngine, int stockIndex, double stockLength, bool stockActive, string stockId)
        {
            var result = new CuttingOptimizationResultStock(stockId, stockLength, stockActive);

            var stockPartCount = cutEngine.GetPartCountOnStock(stockIndex);

            for (var stockPartIndex = 0; stockPartIndex < stockPartCount; stockPartIndex++)
            {
                var partIndex = cutEngine.GetPartIndexOnStock(stockIndex, stockPartIndex);
                result.LinearParts.Add(ResolveCutEngineLinearPart(cutEngine, partIndex));
            }

            var stockRemainingPartCount = cutEngine.GetRemainingPartCountOnStock(stockIndex);

            for (var stockRemainingPartIndex = 0; stockRemainingPartIndex < stockRemainingPartCount; stockRemainingPartIndex++)
            {
                var remainingPartIndex = cutEngine.GetRemainingPartIndexOnStock(stockIndex, stockRemainingPartIndex);
                result.RemainingLinearParts.Add(ResolveCutEngineRemainingPart(cutEngine, remainingPartIndex));
            }

            return result;
        }

        private CuttingOptimizationResultLinearPart ResolveCutEngineLinearPart(CutEngine cutEngine, int partIndex)
        {
            cutEngine.GetResultLinearPart(partIndex, out int partStockIndex, out double partLength, out double partAngleStart, out double partAngleEnd, out double partLocation, out bool partRotated, out bool partFlipped, out string partId);
            return new CuttingOptimizationResultLinearPart(partIndex, partStockIndex, partLength, partAngleStart, partAngleEnd, partLocation, partRotated, partFlipped, partId);
        }

        private CuttingOptimizationResultRemainingPart ResolveCutEngineRemainingPart(CutEngine cutEngine, int remainingPartIndex)
        {
            cutEngine.GetRemainingLinearPart(remainingPartIndex, out int remainingPartStockId, out double remainingPartLength, out double remainingPartLocation, out double remainingPartAngle);
            return new CuttingOptimizationResultRemainingPart(remainingPartIndex, remainingPartStockId, remainingPartLength, remainingPartLocation, remainingPartAngle);
        }

        public class CuttingOptimizationResult
        {
            public bool Success => string.IsNullOrEmpty(Message);
            public string? Message { get; set; }
            public double ElapsedTime { get; set; }
            public List<CuttingOptimizationResultLayout> Layouts { get; set; } = new List<CuttingOptimizationResultLayout>();
        }

        public class CuttingOptimizationResultLayout
        {
            public int Index { get; set; }
            public List<CuttingOptimizationResultStock>? Stocks { get; set; } = new List<CuttingOptimizationResultStock>();
        }

        public class CuttingOptimizationResultStock
        {
            public string? Id { get; set; }
            public double Length { get; set; }
            public bool Active { get; set; }
            public List<CuttingOptimizationResultLinearPart> LinearParts { get; set; } = new List<CuttingOptimizationResultLinearPart>();
            public List<CuttingOptimizationResultRemainingPart> RemainingLinearParts { get; set; } = new List<CuttingOptimizationResultRemainingPart>();

            public CuttingOptimizationResultStock(string? id, double length, bool active)
            {
                Id = id;
                Length = length;
                Active = active;
            }
        }

        public class CuttingOptimizationResultLinearPart
        {
            public int Index { get; set; }
            public int StockIndex { get; set; }
            public double Length { get; set; }
            public double AngleStart { get; set; }
            public double AngleEnd { get; set; }
            public double Location { get; set; }
            public bool Rotated { get; set; }
            public bool Flipped { get; set; }
            public string? Id { get; set; }

            public CuttingOptimizationResultLinearPart(int index, int stockIndex, double length, double angleStart, double angleEnd, double location, bool rotated, bool flipped, string id)
            {
                Index = index;
                StockIndex = stockIndex;
                Length = length;
                AngleStart = angleStart;
                AngleEnd = angleEnd;
                Location = location;
                Rotated = rotated;
                Flipped = flipped;
                Id = id;
            }
        }

        public class CuttingOptimizationResultRemainingPart
        {
            public int Index { get; set; }
            public int StockIndex { get; set; }
            public double Length { get; set; }
            public double Angle { get; set; }
            public double Location { get; set; }

            public CuttingOptimizationResultRemainingPart(int index, int stockIndex, double length, double angle, double location)
            {
                Index = index;
                StockIndex = stockIndex;
                Length = length;
                Angle = angle;
                Location = location;
            }
        }

        public class CuttingOptimizationInputStock
        {
            public string? Id { get; set; }
            public double Length { get; set; }
            public int Count { get; set; }
            public bool IsWaste { get; set; }

            public CuttingOptimizationInputStock(string? id, double length, int count, bool isWaste)
            {
                Id = id;
                Length = length;
                Count = count;
                IsWaste = isWaste;
            }
        }

        public class CuttingOptimizationInputLinearPart
        {
            public string? Id { get; set; }
            public int Count { get; set; }
            public double Length { get; set; }
            public double AngleStart { get; set; } = 0.0;
            public double AngleEnd { get; set; } = 0.0;

            public CuttingOptimizationInputLinearPart(string? id, int count, double length, double angleStart, double angleEnd)
            {
                Id = id;
                Count = count;
                Length = length;
                AngleStart = angleStart;
                AngleEnd = angleEnd;
            }
        }
    }
}
