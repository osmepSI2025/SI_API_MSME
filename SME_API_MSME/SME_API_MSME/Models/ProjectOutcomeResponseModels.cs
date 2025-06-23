public class ResultProjectOutcomeResponse
{
    public int? ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
    public List<ProjectOutcomeResult> Result { get; set; }
}

public class ProjectOutcomeResult
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public List<ProjectOutcomeItem> Items { get; set; }
}

public class ProjectOutcomeItem
{
    public int? OrderIndex { get; set; }
    public string? OutputOutComeName { get; set; }
    public string? YieldTypeName { get; set; }
    public string? MeasureResult { get; set; }
    public int? Target { get; set; }
    public string? CountUnitName { get; set; }
}
