public class ResultProjectAreaResponse
{
    public List<ProjectAreaResult> Result { get; set; } = new List<ProjectAreaResult>();
    public int ResponseCode { get; set; }
    public string ResponseMsg { get; set; }
}

public class ProjectAreaResult
{
    public long ProjectCode { get; set; }
    public string ProjectName { get; set; } 
    public List<ProjectAreaItem> Items { get; set; } = new List<ProjectAreaItem>();
}

public class ProjectAreaItem
{
    public int ProvinceId { get; set; }
    public string ProvinceName { get; set; }
}
