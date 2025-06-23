using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TProjectArea
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int ProvinceId { get; set; }

    public string ProvinceName { get; set; } = null!;

    public virtual MProjectArea Project { get; set; } = null!;
}
