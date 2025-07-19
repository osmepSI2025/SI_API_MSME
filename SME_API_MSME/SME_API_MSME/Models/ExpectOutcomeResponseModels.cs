public class ResultExpectOutcomeResponse
{
    public List<ExpectOutcomeResult> result { get; set; }
    public int responseCode { get; set; }
    public string responseMsg { get; set; }
}

public class ExpectOutcomeResult
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public List<ExpectOutcomeItem> Items { get; set; }
}

public class ExpectOutcomeItem
{
    public int? OrderIndex { get; set; }
    public string? ResultFinalOutcome { get; set; }
    public double? SumBudget { get; set; }
    public List<ExpectOutcomeFinalOutcomeDetail> FinalOutcomeDetail { get; set; }
}

public class ExpectOutcomeFinalOutcomeDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public decimal? ResultValue { get; set; }
}
