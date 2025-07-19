public class ResultBudgetPlanResponse
{
    public List<BudgetPlanResult> result { get; set; }
    public int responseCode { get; set; }
    public string responseMsg { get; set; }
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
    public double? ActivityBudget { get; set; }
    public string? ExpenseTypeName { get; set; }
    public List<BudgetPlanActionResultDetail> ActionResultDetail { get; set; }
}

public class BudgetPlanActionResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public double? TempValue { get; set; }
    public double? ResultValue { get; set; }
}
