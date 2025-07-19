public class ResultProjectActivityResponse
{
    public List<ProjectActivityResult> result { get; set; }
    public int responseCode { get; set; }
    public string responseMsg { get; set; }
}

public class ProjectActivityResult
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public List<ProjectActivityItem> Items { get; set; }
}

public class ProjectActivityItem
{
    public int? OrderIndex { get; set; }
    public string? ItemActivityDetail { get; set; }
    public double? ActivityWeight { get; set; }
    public double? Target { get; set; }
    public string? UnitName { get; set; }
    public List<ActionResultDetail> ActionResultDetail { get; set; }
}

public class ActionResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public double? TempValue { get; set; }
    public double? ResultValue { get; set; }
}
