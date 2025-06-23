public class ResultExpectedOutcomeResultResponse
{
    public List<ExpectedOutcomeResultProject> Result { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
}

public class ExpectedOutcomeResultProject
{
    public long ProjectCode { get; set; }
    public string ProjectName { get; set; }
    public List<ExpectedOutcomeResultItem> Items { get; set; }
}

public class ExpectedOutcomeResultItem
{
    public int? OrderIndex { get; set; }
    public string? ResultFinalOutcome { get; set; }
    public decimal? SumBudget { get; set; }
    public List<ExpectedOutcomeResultFinalOutcomeDetail> FinalOutcomeDetail { get; set; }
}

public class ExpectedOutcomeResultFinalOutcomeDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public decimal? ResultValue { get; set; }
}




