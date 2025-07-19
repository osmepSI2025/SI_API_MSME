public class ResultExpectedOutcomeResultResponse
{
    public List<ExpectedOutcomeResultProject> result { get; set; }
    public int responseCode { get; set; }
    public string responseMsg { get; set; }
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
    public double? SumBudget { get; set; }
    public List<ExpectedOutcomeResultFinalOutcomeDetail> FinalOutcomeDetail { get; set; }
}

public class ExpectedOutcomeResultFinalOutcomeDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public double? ResultValue { get; set; }
}




