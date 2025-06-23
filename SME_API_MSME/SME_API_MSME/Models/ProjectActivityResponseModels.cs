public class ResultProjectActivityResponse
{
    public List<ProjectActivityResult> Result { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
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
    public decimal? ActivityWeight { get; set; }
    public int? Target { get; set; }
    public string? UnitName { get; set; }
    public List<ActionResultDetail> ActionResultDetail { get; set; }
}

public class ActionResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public decimal? TempValue { get; set; }
    public decimal? ResultValue { get; set; }
}
