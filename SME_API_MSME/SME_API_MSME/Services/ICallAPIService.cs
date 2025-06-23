using SME_API_MSME.Models;

namespace SME_API_MSME.Services
{
    public interface ICallAPIService
    {
        Task<string> GetDataApiAsync_Login(MapiInformationModels apiModels);
        Task<ResultApiResponeProject> GetDataApiAsync_Project(MapiInformationModels apiModels, string year);
        Task<ResultProjectAreaResponse> GetDataApiAsync_ProjectArea(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultProjectProductResponse> GetDataApiAsync_ProjectProduct(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultProjectOutcomeResponse> GetDataApiAsync_ProjectOutcome(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultExpectOutcomeResponse> GetDataApiAsync_ExpectOutcome(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultProjectActivityResponse> GetDataApiAsync_ProjectActivity(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultBudgetPlanResponse> GetDataApiAsync_BudgetPlan(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultProductResultResponse> GetDataApiAsync_ProductResult(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultOutcomeResultResponse> GetDataApiAsync_OutcomeResult(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultPerformanceResultResponse> GetDataApiAsync_PerformanceResult(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultDisbursementResultResponse> GetDataApiAsync_DisbursementResult(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultExpectedOutcomeResultResponse> GetDataApiAsync_ExpectedOutcomeResult(MapiInformationModels apiModels, long? pProjectCode);
        Task<ResultEconomicValueResponse> GetDataApiAsync_EconomicValue(MapiInformationModels apiModels, string? pyear);
    }
}
