using Microsoft.AspNetCore.Http;
using SME_API_MSME.Models;
using SME_API_MSME.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SME_API_MSME.Services
{
    public class CallAPIService : ICallAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly string Api_ErrorLog;
        private readonly string Api_SysCode;
        
        private readonly string _FlagDev;
        private readonly IApiInformationRepository _repositoryApi;
        public CallAPIService(HttpClient httpClient, IConfiguration configuration, IApiInformationRepository repositoryApi)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(2); // Increase timeout to 5 minutes (adjust as needed)
            Api_ErrorLog = configuration["Information:Api_ErrorLog"] ?? throw new ArgumentNullException("Api_ErrorLog is missing in appsettings.json");
            Api_SysCode = configuration["Information:Api_SysCode"] ?? throw new ArgumentNullException("Api_SysCode is missing in appsettings.json");
        
            _FlagDev = configuration["Devlopment:FlagDev"] ?? throw new ArgumentNullException("FlagDev is missing in appsettings.json");
            _repositoryApi = repositoryApi ?? throw new ArgumentNullException(nameof(repositoryApi));
        }


        public async Task<string> GetDataApiAsync_Login(MapiInformationModels apiModels)
        {
            if (string.IsNullOrEmpty(apiModels.Urlproduction))
                throw new ArgumentException("Urlproduction cannot be null or empty.");
            if (string.IsNullOrEmpty(apiModels.Username) || string.IsNullOrEmpty(apiModels.Password))
                throw new ArgumentException("Username and Password cannot be null or empty.");

            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, apiModels.Urlproduction);

                // Prepare the payload for the token request
                var payload = new
                {
                    username = apiModels.Username,
                    password = apiModels.Password
                };

                // Serialize the payload to JSON and set it as the request content
                requestJson = JsonSerializer.Serialize(payload, options);
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                // Send the request using the existing _httpClient
                var response = await _httpClient.SendAsync(request);

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Read the response content
                var content = await response.Content.ReadAsStringAsync();

                // Parse the JSON response to extract the token
                using var doc = JsonDocument.Parse(content);
                if (!doc.RootElement.TryGetProperty("access_token", out var tokenElement))
                    throw new Exception("Token not found in response");

                // update token
                var token = tokenElement.GetString();
                _repositoryApi.UpdateAllBearerTokensAsync(token);

                return tokenElement.GetString() ?? throw new Exception("Token is null or empty");
            }
            catch (Exception ex)
            {
                var errorLog = new ErrorLogModels
                {
                    Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    TargetSite = ex.TargetSite?.ToString(),
                    ErrorDate = DateTime.Now,
                    UserName = apiModels.Username,
                    Path = apiModels.Urlproduction,
                    HttpMethod = apiModels.MethodType,
                    RequestData = requestJson,
                    InnerException = ex.InnerException?.ToString(),
                     SystemCode = Api_SysCode,
                    CreatedBy = "system"
                };
                await RecErrorLogApiAsync(apiModels, errorLog);
                throw new Exception("Error in GetDataApiAsync_Login: " + ex.Message, ex);
            }
        }

        public async Task<ResultApiResponeProject> GetDataApiAsync_Project(MapiInformationModels apiModels, string pyear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
                // Call MocData from URL
                var filePath = apiModels.Urldevelopment + "/" + pyear;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultApiResponeProject();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultApiResponeProject>(jsonString, options);

                    return result ?? new ResultApiResponeProject();
                }
                catch (Exception ex)
                {
                    return new ResultApiResponeProject
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_Project: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProjectModels>()
                    }; // Return empty response on error
                }
            }
            else
            {
                try
                {
                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urldevelopment;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{year}", pyear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate,
                                Bearer = x.Bearer,
                            }).First();
                            if (apiParamx == null)
                            {
                                return new ResultApiResponeProject
                                {
                                    responseCode = 500,
                                    responseMsg = "API parameters not found.",
                                    result = new List<ProjectModels>()
                                };
                            }

                            var resultToken = await GetDataApiAsync_Login(apiParamx);
                            token = resultToken;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultApiResponeProject>(content, options);
                    return result ?? new ResultApiResponeProject();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                        SystemCode = Api_SysCode,
                        CreatedBy = "system"
                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultApiResponeProject
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_Project: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProjectModels>()
                    };
                }
            }
        }

        public async Task<ResultProjectAreaResponse> GetDataApiAsync_ProjectArea(MapiInformationModels apiModels, long? pProjectCode,string pYear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
             
                // Call MocData from URL
                    var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultProjectAreaResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultProjectAreaResponse>(jsonString, options);

                    return result ?? new ResultProjectAreaResponse();
                }
                catch (Exception ex)
                {
                    return new ResultProjectAreaResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProjectArea: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProjectAreaResult>()
                    }
                    ;
                }

            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urldevelopment;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}",pYear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                     var content = await response.Content.ReadAsStringAsync();
                    //return JsonSerializer.Deserialize<ResultProjectAreaResponse>(content);
                    var resultapi = JsonSerializer.Deserialize<ResultProjectAreaResponse>(content, options);
                    return resultapi ?? new ResultProjectAreaResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    //  throw new Exception("Error in GetData: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message);
                    return new ResultProjectAreaResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProjectArea: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProjectAreaResult>()
                    }
                   ;
                }
            }

        }
        public async Task<ResultProjectProductResponse> GetDataApiAsync_ProjectProduct(MapiInformationModels apiModels, long? pProjectCode, string pyear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
               
                // Call MocData from URL
                    var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultProjectProductResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultProjectProductResponse>(jsonString, options);

                    return result ?? new ResultProjectProductResponse();
                }
                catch (Exception ex)
                {
                  
                    return new ResultProjectProductResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProjectProduct: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result =new List<ProjectProductResult>()
                    }
                   ;
                }

            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urldevelopment;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}",pyear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var resultapi = JsonSerializer.Deserialize<ResultProjectProductResponse>(content, options);
                    return resultapi ?? new ResultProjectProductResponse();
              
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"


                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultProjectProductResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProjectProduct: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProjectProductResult>()
                    }
                    ;
                }
            }

        }

        public async Task<ResultProjectOutcomeResponse> GetDataApiAsync_ProjectOutcome(MapiInformationModels apiModels, long? pProjectCode, string pyear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
              
                // Call MocData from URL
                    var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultProjectOutcomeResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultProjectOutcomeResponse>(jsonString, options);

                    return result ?? new ResultProjectOutcomeResponse();
                }
                catch (Exception ex)
                {                 
                    return new ResultProjectOutcomeResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProjectOutcome: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProjectOutcomeResult>()
                    };
                }
            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urldevelopment;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}",pyear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
      
                    var resultapi = JsonSerializer.Deserialize<ResultProjectOutcomeResponse>(content, options);
                    return resultapi ?? new ResultProjectOutcomeResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultProjectOutcomeResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProjectOutcome: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProjectOutcomeResult>()
                    };
                }
            }

        }

        public async Task<ResultExpectOutcomeResponse> GetDataApiAsync_ExpectOutcome(MapiInformationModels apiModels, long? pProjectCode,string pyear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
             
                // Call MocData from URL
                    var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultExpectOutcomeResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultExpectOutcomeResponse>(jsonString, options);

                    return result ?? new ResultExpectOutcomeResponse();
                }
                catch (Exception ex)
                {
                  
                    return new ResultExpectOutcomeResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ExpectOutcome: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ExpectOutcomeResult>()
                    };
                }
            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urldevelopment;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}",pyear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                 //   return JsonSerializer.Deserialize<ResultExpectOutcomeResponse>(content);
                    var resultapi = JsonSerializer.Deserialize<ResultExpectOutcomeResponse>(content, options);
                    return resultapi ?? new ResultExpectOutcomeResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultExpectOutcomeResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ExpectOutcome: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ExpectOutcomeResult>()
                    };
                }
            }

        }
        public async Task<ResultProjectActivityResponse> GetDataApiAsync_ProjectActivity(MapiInformationModels apiModels, long? pProjectCode,string pYear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
               

                // Call MocData from URL
                    var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultProjectActivityResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultProjectActivityResponse>(jsonString, options);

                    return result ?? new ResultProjectActivityResponse();
                }
                catch (Exception ex)
                {
                 
                    return new ResultProjectActivityResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProjectActivity: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProjectActivityResult>()
                    };
                }
            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urldevelopment;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}",pYear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                  //  return JsonSerializer.Deserialize<ResultProjectActivityResponse>(content);
                    var resultapi = JsonSerializer.Deserialize<ResultProjectActivityResponse>(content, options);
                    return resultapi ?? new ResultProjectActivityResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultProjectActivityResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProjectActivity: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProjectActivityResult>()
                    };
                }
            }

        }
        public async Task<ResultBudgetPlanResponse> GetDataApiAsync_BudgetPlan(MapiInformationModels apiModels, long? pProjectCode,string pYear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
               

                // Call MocData from URL
                var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultBudgetPlanResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultBudgetPlanResponse>(jsonString, options);

                    return result ?? new ResultBudgetPlanResponse();
                }
                catch (Exception ex)
                {
                 
                    return new ResultBudgetPlanResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_BudgetPlan: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result =new List<BudgetPlanResult>()
                    };
                }
            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urlproduction;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}", pYear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                  //  return JsonSerializer.Deserialize<ResultBudgetPlanResponse>(content);
                    var resultapi = JsonSerializer.Deserialize<ResultBudgetPlanResponse>(content, options);
                    return resultapi ?? new ResultBudgetPlanResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultBudgetPlanResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_BudgetPlan: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<BudgetPlanResult>()
                    };
                }
            }

        }
        public async Task<ResultProductResultResponse> GetDataApiAsync_ProductResult(MapiInformationModels apiModels, long? pProjectCode,string pYear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
                

                // Call MocData from URL
                    var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultProductResultResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultProductResultResponse>(jsonString, options);

                    return result ?? new ResultProductResultResponse();
                }
                catch (Exception ex)
                {
                  
                    return new ResultProductResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProductResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProductResultProject>()
                    };
                }

            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urlproduction;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}",pYear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    //return JsonSerializer.Deserialize<ResultProductResultResponse>(content);
                    var resultapi = JsonSerializer.Deserialize<ResultProductResultResponse>(content, options);
                    return resultapi ?? new ResultProductResultResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultProductResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ProductResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ProductResultProject>()
                    }
                    ;
                }
            }

        }


        public async Task<ResultOutcomeResultResponse> GetDataApiAsync_OutcomeResult(MapiInformationModels apiModels, long? pProjectCode,string pYear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
                
                // Call MocData from URL
                    var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultOutcomeResultResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultOutcomeResultResponse>(jsonString, options);

                    return result ?? new ResultOutcomeResultResponse();
                }
                catch (Exception ex)
                {
                   // return new ResultOutcomeResultResponse();
                    return new ResultOutcomeResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_OutcomeResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<OutcomeResultProject>()
                    };
                }
            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urldevelopment;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}",pYear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                 //   return JsonSerializer.Deserialize<ResultOutcomeResultResponse>(content);
                    var resultapi = JsonSerializer.Deserialize<ResultOutcomeResultResponse>(content, options);
                    return resultapi ?? new ResultOutcomeResultResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultOutcomeResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_OutcomeResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<OutcomeResultProject>()
                    };
                }
            }

        }


        public async Task<ResultPerformanceResultResponse> GetDataApiAsync_PerformanceResult(MapiInformationModels apiModels, long? pProjectCode,string pYear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
              
                    var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultPerformanceResultResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultPerformanceResultResponse>(jsonString, options);

                    return result ?? new ResultPerformanceResultResponse();
                }
                catch (Exception ex)
                {

                    return new ResultPerformanceResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_PerformanceResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<PerformanceResultProject>()
                    };
                }

            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urlproduction;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}", pYear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
               //     return JsonSerializer.Deserialize<ResultPerformanceResultResponse>(content);.
                        var resultapi = JsonSerializer.Deserialize<ResultPerformanceResultResponse>(content, options);
                    return resultapi ?? new ResultPerformanceResultResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultPerformanceResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_PerformanceResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<PerformanceResultProject>()
                    };
                }
            }

        }

        public async Task<ResultDisbursementResultResponse> GetDataApiAsync_DisbursementResult(MapiInformationModels apiModels, long? pProjectCode,string pYear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
               
                // Call MocData from URL
                var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultDisbursementResultResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultDisbursementResultResponse>(jsonString, options);

                    return result ?? new ResultDisbursementResultResponse();
                }
                catch (Exception ex)
                {

                    return new ResultDisbursementResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_DisbursementResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<DisbursementResultProject>()
                    };
                }

            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urlproduction;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}",pYear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                 //   return JsonSerializer.Deserialize<ResultDisbursementResultResponse>(content);
                    var resultapi = JsonSerializer.Deserialize<ResultDisbursementResultResponse>(content, options);
                    return resultapi ?? new ResultDisbursementResultResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    //throw new Exception("Error in GetData: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message);
                    return new ResultDisbursementResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_DisbursementResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<DisbursementResultProject>()
                    }; // Return empty response on error

                }
            }

        }

        public async Task<ResultExpectedOutcomeResultResponse> GetDataApiAsync_ExpectedOutcomeResult(MapiInformationModels apiModels, long? pProjectCode,string pYear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
               

                // Call MocData from URL
                    var filePath = apiModels.Urldevelopment+"/"+ pProjectCode;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultExpectedOutcomeResultResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultExpectedOutcomeResultResponse>(jsonString, options);

                    return result ?? new ResultExpectedOutcomeResultResponse();
                }
                catch (Exception ex)
                {
               
                    return new ResultExpectedOutcomeResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ExpectedOutcomeResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ExpectedOutcomeResultProject>()
                    };
                }

            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urldevelopment;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}",pYear);
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                //    return JsonSerializer.Deserialize<ResultExpectedOutcomeResultResponse>(content);
                    var resultapi = JsonSerializer.Deserialize<ResultExpectedOutcomeResultResponse>(content, options);
                    return resultapi ?? new ResultExpectedOutcomeResultResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                         SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultExpectedOutcomeResultResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_ExpectedOutcomeResult: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<ExpectedOutcomeResultProject>()
                    };
                }
            }

        }

        public async Task<ResultEconomicValueResponse> GetDataApiAsync_EconomicValue(MapiInformationModels apiModels, long? pProjectCode, int? pYear)
        {
            string requestJson = "";
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // ไม่สน case ของ field name
                WriteIndented = true // เวลา serialize จะสวยงามอ่านง่าย
            };
            if (_FlagDev == "Y")
            {
             

                // Call MocData from URL
                    var filePath = apiModels.Urldevelopment+"/"+ pYear;
                try
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(filePath);
                    if (!response.IsSuccessStatusCode)
                        return new ResultEconomicValueResponse();

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResultEconomicValueResponse>(jsonString, options);

                    return result ?? new ResultEconomicValueResponse();
                }
                catch (Exception ex)
                {
                
                    return new ResultEconomicValueResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_EconomicValue: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<EconomicValueProjectModels>()
                    };
                }

            }
            else
            {
                try
                {

                    var handler = new HttpClientHandler
                    {
                        AllowAutoRedirect = false, // Prevent automatic redirection
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    };
                    var httpClient = new HttpClient(handler);

                    var url = $"{apiModels.Urlproduction}";
                    if (apiModels.MethodType == "POST")
                    {
                        url = apiModels.Urlproduction;
                    }
                    else if (apiModels.MethodType == "GET")
                    {
                        url = $"{apiModels.Urlproduction}";
                    }
                    else
                    {
                        throw new Exception("Method type not supported");
                    }
                    url = url.Replace("{projectCode}", pProjectCode.ToString()).Replace("{year}", pYear.ToString());
                    requestJson = url;
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    if (apiModels.AuthorizationType == "Basic")
                    {
                        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{apiModels.Username}:{apiModels.Password}");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    }
                    else if (apiModels.AuthorizationType == "Bearer")
                    {// check token expire
                        string? token = apiModels.Bearer;
                        if (!string.IsNullOrEmpty(apiModels.Bearer) && IsTokenExpired(apiModels.Bearer))
                        {
                            // throw new Exception("Token expired");
                            var LApi = await _repositoryApi.GetAllAsync(new MapiInformationModels { ServiceNameCode = "user-api" });
                            var apiParamx = LApi.Select(x => new MapiInformationModels
                            {
                                ServiceNameCode = x.ServiceNameCode,
                                ApiKey = x.ApiKey,
                                AuthorizationType = x.AuthorizationType,
                                ContentType = x.ContentType,
                                CreateDate = x.CreateDate,
                                Id = x.Id,
                                MethodType = x.MethodType,
                                ServiceNameTh = x.ServiceNameTh,
                                Urldevelopment = x.Urldevelopment,
                                Urlproduction = x.Urlproduction,
                                Username = x.Username,
                                Password = x.Password,
                                UpdateDate = x.UpdateDate
                                ,
                                Bearer = x.Bearer,
                            }).First(); // ดึงตัวแรกของ List
                            if (apiParamx == null)
                            {
                                return null;
                            }

                            var result = GetDataApiAsync_Login(apiParamx);
                            token = result.Result;
                        }
                        else
                        {
                            token = apiModels.Bearer;
                        }

                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    else if (apiModels.AuthorizationType == "ApiKey")
                    {
                        request.Headers.Add("X-Api-Key", apiModels.ApiKey);
                    }
                    else
                    {
                        throw new Exception("Authorization type not supported");
                    }

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    //return JsonSerializer.Deserialize<ResultEconomicValueResponse>(content);
                    var resultapi = JsonSerializer.Deserialize<ResultEconomicValueResponse>(content, options);
                    return resultapi ?? new ResultEconomicValueResponse();
                }
                catch (Exception ex)
                {
                    var errorLog = new ErrorLogModels
                    {
                        Message = "Function " + apiModels.ServiceNameTh + " " + ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        TargetSite = ex.TargetSite?.ToString(),
                        ErrorDate = DateTime.Now,
                        UserName = apiModels.Username, // ดึงจาก context หรือ session
                        Path = apiModels.Urlproduction,
                        HttpMethod = apiModels.MethodType,
                        RequestData = requestJson, // serialize เป็น JSON
                        InnerException = ex.InnerException?.ToString(),
                        SystemCode = Api_SysCode,
                        CreatedBy = "system"

                    };
                    await RecErrorLogApiAsync(apiModels, errorLog);
                    return new ResultEconomicValueResponse
                    {
                        responseCode = 500,
                        responseMsg = "Error in GetDataApiAsync_EconomicValue: " + ex.Message + " | Inner Exception: " + ex.InnerException?.Message,
                        result = new List<EconomicValueProjectModels>()
                    };
                }
            }

        }


        public async Task RecErrorLogApiAsync(MapiInformationModels apiModels, ErrorLogModels eModels)
        {

            // ✅ เลือก URL ตาม _FlagDev
            string? apiUrl = Api_ErrorLog;



            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

                // ✅ ใส่ API Key ถ้ามี
                if (!string.IsNullOrEmpty(apiModels.ApiKey))
                    request.Headers.Add("X-Api-Key", apiModels.ApiKey);

                // ✅ ใส่ Basic Authentication ถ้ามี Username & Password
                if (!string.IsNullOrEmpty(apiModels.Username) && !string.IsNullOrEmpty(apiModels.Password))
                {
                    var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiModels.Username}:{apiModels.Password}"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                }

                // ✅ แปลง SendData เป็น JSON และแนบไปกับ Body ของ Request
                var jsonData = JsonSerializer.Serialize(eModels);
                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // ✅ Call API และรอผลลัพธ์
                using var response = await _httpClient.SendAsync(request);
                string responseData = await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {

            }
        }
        public async Task<T?> ReadJsonFileAsync<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{filePath}' was not found.");
            }

            var jsonContent = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(jsonContent);
        }
        public bool IsTokenExpired(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null) return true;

            var expiry = jwtToken.ValidTo;
            return expiry < DateTime.UtcNow;
        }
        public async Task<string> GetTokenAsync(string username, string password, string tokenUrl)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);

            // Prepare the payload for the token request
            var payload = new
            {
                username = username,
                password = password
            };

            // Serialize the payload to JSON and set it as the request content
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                // Send the request using the existing _httpClient
                var response = await _httpClient.SendAsync(request);

                // Ensure the response is successful
                response.EnsureSuccessStatusCode();

                // Read the response content
                var content = await response.Content.ReadAsStringAsync();

                // Parse the JSON response to extract the token
                using var doc = JsonDocument.Parse(content);
                var token = doc.RootElement.GetProperty("access_token").GetString();

                return token ?? throw new Exception("Token not found in response");
            }
            catch (Exception ex)
            {
                // Log the error or handle it as needed
                throw new Exception($"Failed to retrieve token: {ex.Message}", ex);
            }
        }

    }
}
