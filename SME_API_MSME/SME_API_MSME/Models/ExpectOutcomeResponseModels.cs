public class ResultExpectOutcomeResponse
{
    public List<ExpectOutcomeResult> Result { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
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
    public decimal? SumBudget { get; set; }
    public List<ExpectOutcomeFinalOutcomeDetail> FinalOutcomeDetail { get; set; }
}

public class ExpectOutcomeFinalOutcomeDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public decimal? ResultValue { get; set; }
}
