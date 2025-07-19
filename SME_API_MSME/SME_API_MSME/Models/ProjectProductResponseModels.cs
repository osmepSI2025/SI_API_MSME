public class ResultProjectProductResponse
{
    public int? responseCode { get; set; }
    public string responseMsg { get; set; }
    public List<ProjectProductResult> result { get; set; }
}

public class ProjectProductResult
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public List<ProjectProductItem> items { get; set; }
}

public class ProjectProductItem
{
    public int? OrderIndex { get; set; }
    public string? OutputOutComeName { get; set; }
    public string? YieldTypeName { get; set; }
    public string? MeasureResult { get; set; }
    public double? Target { get; set; }
    public string? CountUnitName { get; set; }
}
