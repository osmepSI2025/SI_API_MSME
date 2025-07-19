using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TSmeEconomicFactor
{
    public int FactorId { get; set; }

    public int? SheetId { get; set; }

    public int? LoginThaiSmeGp { get; set; }

    public int? ProductsServices { get; set; }

    public int? SmeEntrepreneurs { get; set; }

    public double? RegisterThaiSmeGp { get; set; }

    public int? NewEntrepreneur { get; set; }

    public int? OriginalEntrepreneur { get; set; }

    public int? ImproveLaw { get; set; }

    public string? UpdatedLaw { get; set; }

    public string? BusinessPlan { get; set; }

    public string? TrainingCourse { get; set; }

    public string? CourseName { get; set; }

    public int? PersonnelTrained { get; set; }

    public int? Bds { get; set; }

    public string? BusinessField { get; set; }

    public double? SupportMoney { get; set; }

    public int? AmountMicro { get; set; }

    public int? AmountSmall { get; set; }

    public int? SubsidyMedium { get; set; }

    public int? SubsidyOther { get; set; }

    public int? StoryDeveloped { get; set; }

    public int? BusinessServiceProvider { get; set; }

    public virtual TEconomicValueSheets2? Sheet { get; set; }
}
