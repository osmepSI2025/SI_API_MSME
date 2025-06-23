using System;
using System.Collections.Generic;

namespace SME_API_MSME.Entities;

public partial class TSmeEconomicFactor
{
    public int FactorId { get; set; }

    public int? SheetId { get; set; }

    public decimal? LoginThaiSmeGp { get; set; }

    public decimal? ProductsServices { get; set; }

    public decimal? SmeEntrepreneurs { get; set; }

    public decimal? RegisterThaiSmeGp { get; set; }

    public decimal? NewEntrepreneur { get; set; }

    public decimal? OriginalEntrepreneur { get; set; }

    public decimal? ImproveLaw { get; set; }

    public string? UpdatedLaw { get; set; }

    public string? BusinessPlan { get; set; }

    public string? TrainingCourse { get; set; }

    public string? CourseName { get; set; }

    public decimal? PersonnelTrained { get; set; }

    public decimal? Bds { get; set; }

    public string? BusinessField { get; set; }

    public decimal? SupportMoney { get; set; }

    public decimal? AmountMicro { get; set; }

    public decimal? AmountSmall { get; set; }

    public decimal? SubsidyMedium { get; set; }

    public decimal? SubsidyOther { get; set; }

    public decimal? StoryDeveloped { get; set; }

    public decimal? BusinessServiceProvider { get; set; }

    public virtual TEconomicValueSheets2? Sheet { get; set; }
}
