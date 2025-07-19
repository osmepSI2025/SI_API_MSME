public class ResultDisbursementResultResponse
{
    public List<DisbursementResultProject> result { get; set; }
    public int responseCode { get; set; }
    public string responseMsg { get; set; }
}

public class DisbursementResultProject
{
    public long ProjectCode { get; set; }
    public string ProjectName { get; set; }
    public List<DisbursementResultItem> Items { get; set; }
}

public class DisbursementResultItem
{
    public int? OrderIndex { get; set; }
    public string? ItemActivityDetail { get; set; }
    public double? ActivityBudget { get; set; }
    public string? ExpenseTypeName { get; set; }
    public double? SumEffectValue { get; set; }
    public List<DisbursementActionResultDetail> ActionResultDetail { get; set; }
}

public class DisbursementActionResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public double? TempValue { get; set; }
    public double? EffectValue { get; set; }
}



