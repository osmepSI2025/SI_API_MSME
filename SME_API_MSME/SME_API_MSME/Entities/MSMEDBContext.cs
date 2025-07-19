using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SME_API_MSME.Entities;

public partial class MSMEDBContext : DbContext
{
    public MSMEDBContext()
    {
    }

    public MSMEDBContext(DbContextOptions<MSMEDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MApiInformation> MApiInformations { get; set; }

    public virtual DbSet<MBudgetPlan> MBudgetPlans { get; set; }

    public virtual DbSet<MDisbursementResult> MDisbursementResults { get; set; }

    public virtual DbSet<MEconomicValueProject> MEconomicValueProjects { get; set; }

    public virtual DbSet<MExpectedOutcome> MExpectedOutcomes { get; set; }

    public virtual DbSet<MExpectedOutcomeResult> MExpectedOutcomeResults { get; set; }

    public virtual DbSet<MOutcomeResult> MOutcomeResults { get; set; }

    public virtual DbSet<MPerformanceResult> MPerformanceResults { get; set; }

    public virtual DbSet<MProductResult> MProductResults { get; set; }

    public virtual DbSet<MProject> MProjects { get; set; }

    public virtual DbSet<MProjectArea> MProjectAreas { get; set; }

    public virtual DbSet<MProjectsActivity> MProjectsActivities { get; set; }

    public virtual DbSet<MProjectsOutCome> MProjectsOutComes { get; set; }

    public virtual DbSet<MProjectsProduct> MProjectsProducts { get; set; }

    public virtual DbSet<MScheduledJob> MScheduledJobs { get; set; }

    public virtual DbSet<TBudgeMonthlyPlanDetail> TBudgeMonthlyPlanDetails { get; set; }

    public virtual DbSet<TBudgetPlan> TBudgetPlans { get; set; }

    public virtual DbSet<TDisbursementResult> TDisbursementResults { get; set; }

    public virtual DbSet<TDisbursementResultDetail> TDisbursementResultDetails { get; set; }

    public virtual DbSet<TEconomicPromoted> TEconomicPromoteds { get; set; }

    public virtual DbSet<TEconomicValue> TEconomicValues { get; set; }

    public virtual DbSet<TEconomicValueSheets2> TEconomicValueSheets2s { get; set; }

    public virtual DbSet<TExpectedFinalOutcome> TExpectedFinalOutcomes { get; set; }

    public virtual DbSet<TExpectedFinalOutcomeMonthlyPlan> TExpectedFinalOutcomeMonthlyPlans { get; set; }

    public virtual DbSet<TExpectedOutcomeResult> TExpectedOutcomeResults { get; set; }

    public virtual DbSet<TExpectedOutcomeResultDetail> TExpectedOutcomeResultDetails { get; set; }

    public virtual DbSet<TOutcomeResult> TOutcomeResults { get; set; }

    public virtual DbSet<TOutcomeResultDetail> TOutcomeResultDetails { get; set; }

    public virtual DbSet<TPerformanceResult> TPerformanceResults { get; set; }

    public virtual DbSet<TPerformanceResultDetail> TPerformanceResultDetails { get; set; }

    public virtual DbSet<TProductResultOutput> TProductResultOutputs { get; set; }

    public virtual DbSet<TProductResultOutputDetail> TProductResultOutputDetails { get; set; }

    public virtual DbSet<TProjectActivity> TProjectActivities { get; set; }

    public virtual DbSet<TProjectActivityPlan> TProjectActivityPlans { get; set; }

    public virtual DbSet<TProjectArea> TProjectAreas { get; set; }

    public virtual DbSet<TProjectsOutCome> TProjectsOutComes { get; set; }

    public virtual DbSet<TProjectsProduct> TProjectsProducts { get; set; }

    public virtual DbSet<TSmeEconomicDevelop> TSmeEconomicDevelops { get; set; }

    public virtual DbSet<TSmeEconomicDevelopResult> TSmeEconomicDevelopResults { get; set; }

    public virtual DbSet<TSmeEconomicFactor> TSmeEconomicFactors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.9.155;Database=bluecarg_SME_API_MSME;User Id=sa;Password=Osmep@2025;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AS");

        modelBuilder.Entity<MApiInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MApiInformation");

            entity.ToTable("M_ApiInformation", "SI_MSME");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApiKey).HasMaxLength(150);
            entity.Property(e => e.AuthorizationType).HasMaxLength(50);
            entity.Property(e => e.Bearer).HasColumnType("ntext");
            entity.Property(e => e.ContentType).HasMaxLength(150);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.MethodType).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(150);
            entity.Property(e => e.ServiceNameCode).HasMaxLength(250);
            entity.Property(e => e.ServiceNameTh).HasMaxLength(250);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.Urldevelopment).HasColumnName("URLDevelopment");
            entity.Property(e => e.Urlproduction).HasColumnName("URLProduction");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<MBudgetPlan>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__M_Budget__761ABEF07B49E941");

            entity.ToTable("M_BudgetPlan", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__M_Budget__2F3A4948E3F1D639").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<MDisbursementResult>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__M_Disbur__761ABEF0A2056D36");

            entity.ToTable("M_DisbursementResult", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__M_Disbur__2F3A49480981E1B8").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<MEconomicValueProject>(entity =>
        {
            entity.HasKey(e => new { e.ProjectCode, e.BudgetYear }).HasName("PK__M_Econom__2F3A494928A6812E");

            entity.ToTable("M_EconomicValueProject", "SI_MSME");

            entity.Property(e => e.ProjectName).HasMaxLength(255);
        });

        modelBuilder.Entity<MExpectedOutcome>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF0B3F59FAF");

            entity.ToTable("M_ExpectedOutcome", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__Projects__2F3A4948C337D083").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year)
                .HasMaxLength(50)
                .HasColumnName("year");
        });

        modelBuilder.Entity<MExpectedOutcomeResult>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__M_Expect__761ABEF0187371EB");

            entity.ToTable("M_ExpectedOutcomeResult", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__M_Expect__2F3A4948C6FE4FE2").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<MOutcomeResult>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__M_Outcom__761ABEF082DC74BB");

            entity.ToTable("M_OutcomeResult", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__M_Outcom__2F3A4948A0ECDDD8").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<MPerformanceResult>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__M_Perfor__761ABEF069DA8403");

            entity.ToTable("M_PerformanceResult", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__M_Perfor__2F3A4948F1B01384").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<MProductResult>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__M_Produc__761ABEF03ACBD312");

            entity.ToTable("M_ProductResult", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__M_Produc__2F3A4948368576E1").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<MProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MSME_M_Project");

            entity.ToTable("M_Project", "SI_MSME");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BudgetYear)
                .HasMaxLength(50)
                .HasColumnName("budgetYear");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createDate");
            entity.Property(e => e.DateApprove)
                .HasMaxLength(50)
                .HasColumnName("dateApprove");
            entity.Property(e => e.EndDate)
                .HasMaxLength(50)
                .HasColumnName("endDate");
            entity.Property(e => e.LegalGroupName)
                .HasMaxLength(250)
                .HasColumnName("legalGroupName");
            entity.Property(e => e.MinistryId)
                .HasMaxLength(50)
                .HasColumnName("ministryID");
            entity.Property(e => e.MinistryName)
                .HasMaxLength(255)
                .HasColumnName("ministryName");
            entity.Property(e => e.OrgId).HasColumnName("orgId");
            entity.Property(e => e.OrgName)
                .HasMaxLength(250)
                .HasColumnName("orgName");
            entity.Property(e => e.PlanMessage)
                .HasColumnType("ntext")
                .HasColumnName("planMessage");
            entity.Property(e => e.ProjectBudget)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("projectBudget");
            entity.Property(e => e.ProjectCode).HasColumnName("projectCode");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(250)
                .HasColumnName("projectName");
            entity.Property(e => e.ProjectNameInitials)
                .HasMaxLength(250)
                .HasColumnName("projectNameInitials");
            entity.Property(e => e.ProjectOffBudget)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("projectOffBudget");
            entity.Property(e => e.ProjectPurpose)
                .HasColumnType("ntext")
                .HasColumnName("projectPurpose");
            entity.Property(e => e.ProjectReason)
                .HasColumnType("ntext")
                .HasColumnName("projectReason");
            entity.Property(e => e.ProjectSumBudget)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("projectSumBudget");
            entity.Property(e => e.SmeProjectStatusName)
                .HasMaxLength(50)
                .HasColumnName("smeProjectStatusName");
            entity.Property(e => e.StartDate)
                .HasMaxLength(50)
                .HasColumnName("startDate");
            entity.Property(e => e.TypeBudget)
                .HasMaxLength(50)
                .HasColumnName("typeBudget");
            entity.Property(e => e.TypeResultMsme)
                .HasMaxLength(250)
                .HasColumnName("typeResultMsme");
        });

        modelBuilder.Entity<MProjectArea>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF0C488CDDE");

            entity.ToTable("M_ProjectArea", "SI_MSME");

            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.Year)
                .HasMaxLength(50)
                .HasColumnName("year");
        });

        modelBuilder.Entity<MProjectsActivity>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__M_Projec__761ABEF07163B712");

            entity.ToTable("M_ProjectsActivity", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__M_Projec__2F3A4948D7CBE7DC").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<MProjectsOutCome>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF0EF32CD0E");

            entity.ToTable("M_ProjectsOutCome", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__Projects__2F3A4948B9ADFD06").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year)
                .HasMaxLength(50)
                .HasColumnName("year");
        });

        modelBuilder.Entity<MProjectsProduct>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF0B0CFD0BE");

            entity.ToTable("M_ProjectsProduct", "SI_MSME");

            entity.HasIndex(e => e.ProjectCode, "UQ__Projects__2F3A49480F6893BD").IsUnique();

            entity.Property(e => e.ProjectName).HasMaxLength(255);
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<MScheduledJob>(entity =>
        {
            entity.ToTable("M_ScheduledJobs", "SI_MSME");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JobName).HasMaxLength(150);
        });

        modelBuilder.Entity<TBudgeMonthlyPlanDetail>(entity =>
        {
            entity.HasKey(e => e.MonthlyDetailId).HasName("PK__T_BudgeM__ED7FD5CF1D648D61");

            entity.ToTable("T_BudgeMonthlyPlanDetails", "SI_MSME");

            entity.Property(e => e.MonthName).HasMaxLength(50);

            entity.HasOne(d => d.BudgetPlan).WithMany(p => p.TBudgeMonthlyPlanDetails)
                .HasForeignKey(d => d.BudgetPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_BudgeMo__Budge__114A936A");
        });

        modelBuilder.Entity<TBudgetPlan>(entity =>
        {
            entity.HasKey(e => e.BudgetPlanId).HasName("PK__T_Budget__73D762B9428C6A2B");

            entity.ToTable("T_BudgetPlans", "SI_MSME");

            entity.Property(e => e.ExpenseTypeName).HasMaxLength(100);

            entity.HasOne(d => d.Project).WithMany(p => p.TBudgetPlans)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_BudgetP__Proje__0E6E26BF");
        });

        modelBuilder.Entity<TDisbursementResult>(entity =>
        {
            entity.HasKey(e => e.BudgetActivityId).HasName("PK__T_Disbur__079616F1D553A59D");

            entity.ToTable("T_DisbursementResult", "SI_MSME");

            entity.Property(e => e.ExpenseTypeName).HasMaxLength(200);

            entity.HasOne(d => d.Project).WithMany(p => p.TDisbursementResults)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Disburs__Proje__4D5F7D71");
        });

        modelBuilder.Entity<TDisbursementResultDetail>(entity =>
        {
            entity.HasKey(e => e.MonthlyDisbursementId).HasName("PK__T_Disbur__AA0E9DD2068B300F");

            entity.ToTable("T_DisbursementResultDetail", "SI_MSME");

            entity.Property(e => e.MonthName).HasMaxLength(50);

            entity.HasOne(d => d.BudgetActivity).WithMany(p => p.TDisbursementResultDetails)
                .HasForeignKey(d => d.BudgetActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Disburs__Budge__503BEA1C");
        });

        modelBuilder.Entity<TEconomicPromoted>(entity =>
        {
            entity.HasKey(e => e.PromotedId).HasName("PK__T_Econom__8E36C53FA7228E83");

            entity.ToTable("T_EconomicPromoted", "SI_MSME");

            entity.Property(e => e.AgribusinessBranch).HasMaxLength(500);
            entity.Property(e => e.ProductionBranch).HasMaxLength(500);
            entity.Property(e => e.ServeBranch).HasMaxLength(500);
            entity.Property(e => e.TradeBranch).HasMaxLength(500);

            entity.HasOne(d => d.Sheet).WithMany(p => p.TEconomicPromoteds)
                .HasForeignKey(d => d.SheetId)
                .HasConstraintName("FK__T_Economi__Sheet__45544755");
        });

        modelBuilder.Entity<TEconomicValue>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.EconomicValueId });

            entity.ToTable("T_EconomicValues", "SI_MSME");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
        });

        modelBuilder.Entity<TEconomicValueSheets2>(entity =>
        {
            entity.HasKey(e => e.SheetId).HasName("PK__T_Econom__30B273E8BCF8BCE6");

            entity.ToTable("T_EconomicValueSheets2", "SI_MSME");

            entity.Property(e => e.Province).HasMaxLength(255);
        });

        modelBuilder.Entity<TExpectedFinalOutcome>(entity =>
        {
            entity.HasKey(e => e.FinalOutcomeId).HasName("PK__T_Expect__7DEC8211519E2450");

            entity.ToTable("T_ExpectedFinalOutcome", "SI_MSME");

            entity.HasOne(d => d.Project).WithMany(p => p.TExpectedFinalOutcomes)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Expecte__Proje__4BAC3F29");
        });

        modelBuilder.Entity<TExpectedFinalOutcomeMonthlyPlan>(entity =>
        {
            entity.HasKey(e => e.MonthlyPlanId).HasName("PK__T_Expect__D31169BCC711FB80");

            entity.ToTable("T_ExpectedFinalOutcomeMonthlyPlan", "SI_MSME");

            entity.Property(e => e.MonthName).HasMaxLength(50);
            entity.Property(e => e.ResultValue).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.FinalOutcome).WithMany(p => p.TExpectedFinalOutcomeMonthlyPlans)
                .HasForeignKey(d => d.FinalOutcomeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Expecte__Final__4E88ABD4");
        });

        modelBuilder.Entity<TExpectedOutcomeResult>(entity =>
        {
            entity.HasKey(e => e.OutcomeId).HasName("PK__T_Expect__113E6A1C71EF7ADE");

            entity.ToTable("T_ExpectedOutcomeResult", "SI_MSME");

            entity.Property(e => e.ResultFinalOutcome).HasMaxLength(500);

            entity.HasOne(d => d.Project).WithMany(p => p.TExpectedOutcomeResults)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Expecte__Proje__30C33EC3");
        });

        modelBuilder.Entity<TExpectedOutcomeResultDetail>(entity =>
        {
            entity.HasKey(e => e.MonthlyOutcomeId).HasName("PK__T_Expect__E26C43B85F204F64");

            entity.ToTable("T_ExpectedOutcomeResultDetail", "SI_MSME");

            entity.Property(e => e.MonthName).HasMaxLength(50);

            entity.HasOne(d => d.Outcome).WithMany(p => p.TExpectedOutcomeResultDetails)
                .HasForeignKey(d => d.OutcomeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Expecte__Outco__339FAB6E");
        });

        modelBuilder.Entity<TOutcomeResult>(entity =>
        {
            entity.HasKey(e => e.OutcomeId).HasName("PK__T_Outcom__113E6A1C852D23AF");

            entity.ToTable("T_OutcomeResult", "SI_MSME");

            entity.Property(e => e.UnitName).HasMaxLength(100);
            entity.Property(e => e.YieldTypeName).HasMaxLength(100);

            entity.HasOne(d => d.Project).WithMany(p => p.TOutcomeResults)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Outcome__Proje__1F98B2C1");
        });

        modelBuilder.Entity<TOutcomeResultDetail>(entity =>
        {
            entity.HasKey(e => e.MonthlyOutcomeResultId).HasName("PK__T_Outcom__47C15A14E2DE0B44");

            entity.ToTable("T_OutcomeResultDetails", "SI_MSME");

            entity.Property(e => e.MonthName).HasMaxLength(50);

            entity.HasOne(d => d.Outcome).WithMany(p => p.TOutcomeResultDetails)
                .HasForeignKey(d => d.OutcomeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Outcome__Outco__22751F6C");
        });

        modelBuilder.Entity<TPerformanceResult>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__T_Perfor__45F4A791BC24EE8D");

            entity.ToTable("T_PerformanceResult", "SI_MSME");

            entity.Property(e => e.ExpenseTypeName).HasMaxLength(100);
            entity.Property(e => e.UnitName).HasMaxLength(100);

            entity.HasOne(d => d.Project).WithMany(p => p.TPerformanceResults)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Perform__Proje__17036CC0");
        });

        modelBuilder.Entity<TPerformanceResultDetail>(entity =>
        {
            entity.HasKey(e => e.MonthlyActivityResultId).HasName("PK__T_Perfor__968513E88204C156");

            entity.ToTable("T_PerformanceResultDetails", "SI_MSME");

            entity.Property(e => e.MonthName).HasMaxLength(50);

            entity.HasOne(d => d.Activity).WithMany(p => p.TPerformanceResultDetails)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Perform__Activ__19DFD96B");
        });

        modelBuilder.Entity<TProductResultOutput>(entity =>
        {
            entity.HasKey(e => e.OutputId).HasName("PK__T_Produc__CE7609665830B325");

            entity.ToTable("T_ProductResultOutputs", "SI_MSME");

            entity.Property(e => e.OutputOutcomeName).HasMaxLength(500);
            entity.Property(e => e.UnitName).HasMaxLength(100);
            entity.Property(e => e.YieldTypeName).HasMaxLength(100);

            entity.HasOne(d => d.Project).WithMany(p => p.TProductResultOutputs)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Product__Proje__74AE54BC");
        });

        modelBuilder.Entity<TProductResultOutputDetail>(entity =>
        {
            entity.HasKey(e => e.MonthlyResultId).HasName("PK__T_Produc__1470028A156CD976");

            entity.ToTable("T_ProductResultOutputDetail", "SI_MSME");

            entity.Property(e => e.MonthName).HasMaxLength(50);

            entity.HasOne(d => d.Output).WithMany(p => p.TProductResultOutputDetails)
                .HasForeignKey(d => d.OutputId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Product__Outpu__778AC167");
        });

        modelBuilder.Entity<TProjectActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__T_Projec__45F4A791A761614B");

            entity.ToTable("T_ProjectActivities", "SI_MSME");

            entity.Property(e => e.UnitName).HasMaxLength(100);

            entity.HasOne(d => d.Project).WithMany(p => p.TProjectActivities)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Project__Proje__5441852A");
        });

        modelBuilder.Entity<TProjectActivityPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__T_Projec__755C22B7BC6F5892");

            entity.ToTable("T_ProjectActivityPlans", "SI_MSME");

            entity.Property(e => e.MonthName).HasMaxLength(50);

            entity.HasOne(d => d.Activity).WithMany(p => p.TProjectActivityPlans)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Project__Activ__571DF1D5");
        });

        modelBuilder.Entity<TProjectArea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__T_Projec__3214EC070913E7CF");

            entity.ToTable("T_ProjectArea", "SI_MSME");

            entity.Property(e => e.ProvinceName).HasMaxLength(100);

            entity.HasOne(d => d.Project).WithMany(p => p.TProjectAreas)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Project__Proje__403A8C7D");
        });

        modelBuilder.Entity<TProjectsOutCome>(entity =>
        {
            entity.HasKey(e => e.OutcomeId).HasName("PK__T_Projec__113E6A1CA43787F6");

            entity.ToTable("T_ProjectsOutCome", "SI_MSME");

            entity.Property(e => e.CountUnitName)
                .HasMaxLength(100)
                .HasColumnName("countUnitName");

            entity.HasOne(d => d.Project).WithMany(p => p.TProjectsOutComes)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Project__Proje__45F365D3");
        });

        modelBuilder.Entity<TProjectsProduct>(entity =>
        {
            entity.HasKey(e => e.OutputId).HasName("PK__T_Projec__CE7609663F8B3EA2");

            entity.ToTable("T_ProjectsProduct", "SI_MSME");

            entity.Property(e => e.CountUnitName)
                .HasMaxLength(100)
                .HasColumnName("countUnitName");

            entity.HasOne(d => d.Project).WithMany(p => p.TProjectsProducts)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__T_Project__Proje__4316F928");
        });

        modelBuilder.Entity<TSmeEconomicDevelop>(entity =>
        {
            entity.HasKey(e => e.DevelopId).HasName("PK__T_SmeEco__9B9CE55A1665D339");

            entity.ToTable("T_SmeEconomicDevelops", "SI_MSME");

            entity.Property(e => e.BusinessBranch).HasMaxLength(255);
            entity.Property(e => e.Cluster).HasMaxLength(255);

            entity.HasOne(d => d.Sheet).WithMany(p => p.TSmeEconomicDevelops)
                .HasForeignKey(d => d.SheetId)
                .HasConstraintName("FK__T_SmeEcon__Sheet__4830B400");
        });

        modelBuilder.Entity<TSmeEconomicDevelopResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__T_SmeEco__97690208FFA14966");

            entity.ToTable("T_SmeEconomicDevelopResults", "SI_MSME");

            entity.Property(e => e.FieldBusinessInvestment).HasMaxLength(500);

            entity.HasOne(d => d.Sheet).WithMany(p => p.TSmeEconomicDevelopResults)
                .HasForeignKey(d => d.SheetId)
                .HasConstraintName("FK__T_SmeEcon__Sheet__4DE98D56");
        });

        modelBuilder.Entity<TSmeEconomicFactor>(entity =>
        {
            entity.HasKey(e => e.FactorId).HasName("PK__T_SmeEco__E733AADDFD12C5CD");

            entity.ToTable("T_SmeEconomicFactors", "SI_MSME");

            entity.Property(e => e.BusinessField).HasMaxLength(255);
            entity.Property(e => e.BusinessPlan).HasMaxLength(255);
            entity.Property(e => e.CourseName).HasMaxLength(255);
            entity.Property(e => e.TrainingCourse).HasMaxLength(255);
            entity.Property(e => e.UpdatedLaw).HasMaxLength(255);

            entity.HasOne(d => d.Sheet).WithMany(p => p.TSmeEconomicFactors)
                .HasForeignKey(d => d.SheetId)
                .HasConstraintName("FK__T_SmeEcon__Sheet__4B0D20AB");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
