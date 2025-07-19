public class ResultOutcomeResultResponse
{
    public List<OutcomeResultProject> result { get; set; }
    public int responseCode { get; set; }
    public string responseMsg { get; set; }
}

public class OutcomeResultProject
{
    public long ProjectCode { get; set; }
    public string ProjectName { get; set; }
    public List<OutcomeResultItem> Items { get; set; }
}

public class OutcomeResultItem
{
    public int? OrderIndex { get; set; }
    public string? OutputOutComeName { get; set; }
    public string? YieldTypeName { get; set; }
    public double? Target { get; set; }
    public string? UnitName { get; set; }
    public List<OutcomeResultDetail> OutcomeResult { get; set; }
}

public class OutcomeResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public double? ResultOfYear { get; set; }
    public double? ResultOffEffect { get; set; }
    public string? Detail { get; set; }
    public string? Problem { get; set; }
    public string? Solution { get; set; }
}

