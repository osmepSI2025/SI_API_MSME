using SME_API_MSME.Models;

namespace SME_API_MSME.Services
{
    public interface ICallAPIService
    {
        Task<string> GetDataApiAsync_Login(MapiInformationModels apiModels);
        Task<ResultApiResponeProject> GetDataApiAsync_Project(MapiInformationModels apiModels, string year);
        Task<ResultProjectAreaResponse> GetDataApiAsync_ProjectArea(MapiInformationModels apiModels, long? pProjectCode,string pyear);
        Task<ResultProjectProductResponse> GetDataApiAsync_ProjectProduct(MapiInformationModels apiModels, long? pProjectCode, string pyear);
        Task<ResultProjectOutcomeResponse> GetDataApiAsync_ProjectOutcome(MapiInformationModels apiModels, long? pProjectCode,string pyear);
        Task<ResultExpectOutcomeResponse> GetDataApiAsync_ExpectOutcome(MapiInformationModels apiModels, long? pProjectCode,string pyear);
        Task<ResultProjectActivityResponse> GetDataApiAsync_ProjectActivity(MapiInformationModels apiModels, long? pProjectCode,string pYear);
        Task<ResultBudgetPlanResponse> GetDataApiAsync_BudgetPlan(MapiInformationModels apiModels, long? pProjectCode,string pYear);
        Task<ResultProductResultResponse> GetDataApiAsync_ProductResult(MapiInformationModels apiModels, long? pProjectCode,string pYear);
        Task<ResultOutcomeResultResponse> GetDataApiAsync_OutcomeResult(MapiInformationModels apiModels, long? pProjectCode,string pYear);
        Task<ResultPerformanceResultResponse> GetDataApiAsync_PerformanceResult(MapiInformationModels apiModels, long? pProjectCode,string pYear);
        Task<ResultDisbursementResultResponse> GetDataApiAsync_DisbursementResult(MapiInformationModels apiModels, long? pProjectCode, string pYear);
        Task<ResultExpectedOutcomeResultResponse> GetDataApiAsync_ExpectedOutcomeResult(MapiInformationModels apiModels, long? pProjectCode,string pYear);
        Task<ResultEconomicValueResponse> GetDataApiAsync_EconomicValue(MapiInformationModels apiModels, long? pProjectCode,int? pyear);
    }
}
