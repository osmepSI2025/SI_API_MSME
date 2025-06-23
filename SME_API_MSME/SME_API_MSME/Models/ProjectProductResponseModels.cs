public class ResultProjectProductResponse
{
    public int? ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
    public List<ProjectProductResult> Result { get; set; }
}

public class ProjectProductResult
{
    public long ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public List<ProjectProductItem> Items { get; set; }
}

public class ProjectProductItem
{
    public int? OrderIndex { get; set; }
    public string? OutputOutComeName { get; set; }
    public string? YieldTypeName { get; set; }
    public string? MeasureResult { get; set; }
    public int? Target { get; set; }
    public string? CountUnitName { get; set; }
}
