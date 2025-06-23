using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TEconomicValue
{
    public int Id { get; set; }

    public int EconomicValueId { get; set; }

    public long? ProjectCode { get; set; }

    public decimal? MicroEnd { get; set; }

    public decimal? MicroNext { get; set; }

    public decimal? SmallEnd { get; set; }

    public decimal? SmallNext { get; set; }

    public decimal? MediumEnd { get; set; }

    public decimal? MediumNext { get; set; }

    public decimal? OtherEnd { get; set; }

    public decimal? OtherNext { get; set; }
}
