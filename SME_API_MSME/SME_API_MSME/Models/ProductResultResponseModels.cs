public class ResultProductResultResponse
{
    public List<ProductResultProject> Result { get; set; }
    public int ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
}

public class ProductResultProject
{
    public long ProjectCode { get; set; }
    public string ProjectName { get; set; }
    public List<ProductResultItem> Items { get; set; }
}

public class ProductResultItem
{
    public int? OrderIndex { get; set; }
    public string? OutputOutComeName { get; set; }
    public string? YieldTypeName { get; set; }
    public int? Target { get; set; }
    public string? UnitName { get; set; }
    public List<ProductResultDetail> ProductResult { get; set; }
}

public class ProductResultDetail
{
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public decimal? ResultOfYear { get; set; }
    public decimal? ResultOffEffect { get; set; }
    public string? Detail { get; set; }
    public string? Problem { get; set; }
    public string? Solution { get; set; }
}
