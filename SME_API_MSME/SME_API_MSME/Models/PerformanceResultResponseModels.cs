public class ResultPerformanceResultResponse
{
    public List<PerformanceResultProject> Result { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
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
    public decimal? ActivityWeight { get; set; }
    public int? Target { get; set; }
    public string? UnitName { get; set; }
    public decimal? SumEffectValue { get; set; }
    public List<PerformanceActionResultDetail> ActionResultDetail { get; set; }
}

public class PerformanceActionResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public decimal? EffectValue { get; set; }
    public decimal? TempValue { get; set; }
}


