using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TEconomicValue
{
    public int Id { get; set; }

    public int EconomicValueId { get; set; }

    public long? ProjectCode { get; set; }

    public double? MicroEnd { get; set; }

    public double? MicroNext { get; set; }

    public double? SmallEnd { get; set; }

    public double? SmallNext { get; set; }

    public double? MediumEnd { get; set; }

    public double? MediumNext { get; set; }

    public double? OtherEnd { get; set; }

    public double? OtherNext { get; set; }
}
