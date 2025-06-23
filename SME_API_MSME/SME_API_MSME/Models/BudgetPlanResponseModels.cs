public class ResultBudgetPlanResponse
{
    public List<BudgetPlanResult> Result { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
}

public class BudgetPlanResult
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public List<BudgetPlanItem> Items { get; set; }
}

public class BudgetPlanItem
{
    public int? OrderIndex { get; set; }
    public string? ItemActivityDetail { get; set; }
    public decimal? ActivityBudget { get; set; }
    public string? ExpenseTypeName { get; set; }
    public List<BudgetPlanActionResultDetail> ActionResultDetail { get; set; }
}

public class BudgetPlanActionResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public decimal? TempValue { get; set; }
    public decimal? ResultValue { get; set; }
}
