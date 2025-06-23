public class ResultDisbursementResultResponse
{
    public List<DisbursementResultProject> Result { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
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
    public decimal? ActivityBudget { get; set; }
    public string? ExpenseTypeName { get; set; }
    public decimal? SumEffectValue { get; set; }
    public List<DisbursementActionResultDetail> ActionResultDetail { get; set; }
}

public class DisbursementActionResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public decimal? TempValue { get; set; }
    public decimal? EffectValue { get; set; }
}



