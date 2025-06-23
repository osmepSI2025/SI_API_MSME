namespace SME_API_MSME.Models
{
    public class ProjectModels
    {
        public long ProjectCode { get; set; }

        public string? BudgetYear { get; set; }

        public string? DateApprove { get; set; }

        public long? OrgId { get; set; }

        public string? OrgName { get; set; }

        public decimal? ProjectBudget { get; set; }

        public decimal? ProjectOffBudget { get; set; }

        public decimal? ProjectSumBudget { get; set; }

        public string? SmeProjectStatusName { get; set; }

        public string? LegalGroupName { get; set; }

        public string? ProjectName { get; set; }

        public string? ProjectNameInitials { get; set; }

        public string? ProjectReason { get; set; }

        public string? ProjectPurpose { get; set; }

        public string? TypeBudget { get; set; }

        public string? TypeResultMsme { get; set; }

        public string? PlanMessage { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }
    }
 

    public class ResultApiResponeProject
    {
        public List<ProjectModels> Result { get; set; }
        
        public int ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
    }
}
