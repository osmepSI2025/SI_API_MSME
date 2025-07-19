public class ResultPerformanceResultResponse
{
    public List<PerformanceResultProject> result { get; set; }
    public int responseCode { get; set; }
    public string responseMsg { get; set; }
}

public class PerformanceResultProject
{
    public long ProjectCode { get; set; }
    public string ProjectName { get; set; }
    public List<PerformanceResultItem> Items { get; set; }
}

public class PerformanceResultItem
{
    public int? OrderIndex { get; set; }
    public string? ItemActivityDetail { get; set; }
    public string? ExpenseTypeName { get; set; }
    public double? ActivityWeight { get; set; }
    public double? Target { get; set; }
    public string? UnitName { get; set; }
    public double? SumEffectValue { get; set; }
    public List<PerformanceActionResultDetail> ActionResultDetail { get; set; }
}

public class PerformanceActionResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public double? EffectValue { get; set; }
    public double? TempValue { get; set; }
}


