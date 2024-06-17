using System.Text.Json;
using MO.MODBClient.DTOs;
using MO.MODBClient.HTTP;

namespace MO.MODBClient{
    public class MODBClient : IMODBClient, IDisposable
    {
        HTTPClient _httpClient;
        string _baseUrl;
        string Endpoint(string resource) => $"{_baseUrl}/{resource}";
        public MODBClient(string host, string apikey, string version = "v1"){
            if(string.IsNullOrEmpty(host))
                throw new ArgumentException(paramName: "host", message: "Invalid host, can not be null or empty");
            if(string.IsNullOrEmpty(apikey))
                throw new ArgumentException(paramName: "apikey", message: "Invalid apikey, can not be null or empty");
            _baseUrl = $"api/{version}";
            _httpClient = new HTTPClient(host, new Dictionary<string, string>(){{"ApiKey", apikey} });
        }

        public async Task<IEnumerable<string>> GetDBsAsync(CancellationToken cs = default)
        {
            try{
                return await _httpClient.GetAsync(
                    endpoint: Endpoint("databases"),
                    func: stream => JsonSerializer.Deserialize<MODBResponse<IEnumerable<string>>>(stream).Result, 
                    cancellationToken: cs);
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task<DBInformation> GetDBAsync(string name, CancellationToken cs = default)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentException(paramName: "name", message: "Database name, can not be null or empty");
            try{
                return await _httpClient.GetAsync(
                    endpoint: Endpoint("databases/{0}"),
                    func: stream => JsonSerializer.Deserialize<MODBResponse<DBInformation>>(stream).Result, 
                    cancellationToken: cs,
                    routeParams: name);
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task CreateDBAsync(string name, CancellationToken cs = default)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentException(paramName: "name", message: "Database name, can not be null or empty");
            try{
                await _httpClient.PostAsync(
                    endpoint: Endpoint("databases"),
                    action: stream => {},
                    body: new StringContent(JsonSerializer.Serialize(new {name = name})),
                    cancellationToken: cs
                );
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task DeleteDBAsync(string db, CancellationToken cs = default)
        {
            if(string.IsNullOrEmpty(db))
                throw new ArgumentException(paramName: "db", message: "Database name, can not be null or empty");
            try{
                await _httpClient.DeleteAsync(
                    endpoint: Endpoint("databases/{0}"),
                    action: stream => {}, 
                    cancellationToken: cs,
                    routeParams: new string[]{db});
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task SetAsync(string db, string key, string type, string value, IndexItem[] indices = null, bool? createDb = true, CancellationToken cs = default)
        {
            if(string.IsNullOrEmpty(db))
                throw new ArgumentException(paramName: "db", message: "Database name, can not be null or empty");
            if(string.IsNullOrEmpty(key))
                throw new ArgumentException(paramName: "key", message: "key, can not be null or empty");
            if(string.IsNullOrEmpty(type))
                throw new ArgumentException(paramName: "type", message: "type, can not be null or empty");
            if(string.IsNullOrEmpty(value))
                throw new ArgumentException(paramName: "value", message: "value, can not be null or empty");

            try{
                await _httpClient.PostAsync(
                    endpoint: Endpoint("databases/{0}/values"),
                    action: stream => {},
                    body: new StringContent(JsonSerializer.Serialize(new {key = key, type = type, value = value, indices = indices, createDb = createDb})), 
                    cancellationToken: cs,
                    routeParams: db);
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task<T> GetAsync<T>(string db, string key, CancellationToken cs = default)
        {
            if(string.IsNullOrEmpty(db))
                throw new ArgumentException(paramName: "db", message: "Database name, can not be null or empty");
            if(string.IsNullOrEmpty(key))
                throw new ArgumentException(paramName: "key", message: "key, can not be null or empty");
            var  request = new HttpRequestMessage(){
                RequestUri = new Uri($"{_httpClient.Host}/{Endpoint($"databases/{db}/values/{key}")}"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("result-type","json");
            try{
                return await _httpClient.SendAsync(
                    request: request,
                    func: stream => JsonSerializer.Deserialize<MODBResponse<T>>(stream).Result, 
                    cancellationToken: cs);
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task<string> GetStringAsync(string db, string key, CancellationToken cs = default)
        {
            if(string.IsNullOrEmpty(db))
                throw new ArgumentException(paramName: "db", message: "Database name, can not be null or empty");
            if(string.IsNullOrEmpty(key))
                throw new ArgumentException(paramName: "key", message: "key, can not be null or empty");
            try{
                return await _httpClient.GetAsync(
                    endpoint: Endpoint("databases/{0}/values/{1}"),
                    func: stream => JsonSerializer.Deserialize<MODBResponse<string>>(stream).Result, 
                    cancellationToken: cs,
                    routeParams: new string[]{db, key});
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task DeleteAsync(string db, string key, CancellationToken cs = default)
        {
            if(string.IsNullOrEmpty(db))
                throw new ArgumentException(paramName: "db", message: "Database name, can not be null or empty");
            if(string.IsNullOrEmpty(key))
                throw new ArgumentException(paramName: "key", message: "key, can not be null or empty");
            try{
                await _httpClient.DeleteAsync(
                    endpoint: Endpoint("databases/{0}/values/{1}"),
                    action: stream => {}, 
                    cancellationToken: cs,
                    routeParams: new string[]{db, key});
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task<PagedList<T>> GetAsync<T>(string db, string index, CompareOperators compareOperator, string value, int page = 1, int pageSize = 10, CancellationToken cs = default)
        {
            if(string.IsNullOrEmpty(db))
                throw new ArgumentException(paramName: "db", message: "Database name, can not be null or empty");
            if(string.IsNullOrEmpty(index))
                throw new ArgumentException(paramName: "index", message: "Index, can not be null or empty");
            if(page <= 0)
                throw new ArgumentException(paramName: "page", message: "Page, must be greater than 0");
            if(pageSize <= 0)
                throw new ArgumentException(paramName: "pageSize", message: "Page size, must be greater than 0");
            var queryStringParams = new Dictionary<string, string>(){
                {"indexName", $"{index}"},
                {"compareOperator", $"{compareOperator}"},
                {"value", $"{value}"},
                {"page", $"{page}"},
                {"pageSize", $"{pageSize}"}
            };
            var  request = new HttpRequestMessage(){
                RequestUri = new Uri($"{_httpClient.Host}/{Endpoint($"databases/{db}/filter")}{(queryStringParams == null || !queryStringParams.Any() ? "" : $"?{string.Join("&", queryStringParams.Select(parm => $"{parm.Key}={parm.Value}"))}")}"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("result-type","json");
            try{
                return await _httpClient.SendAsync(
                    request: request,
                    func: stream => JsonSerializer.Deserialize<MODBResponse<PagedList<T>>>(stream).Result, 
                    cancellationToken: cs);
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task<PagedList<string>> GetStringAsync(string db, string index, CompareOperators compareOperator, string value, int page = 1, int pageSize = 10, CancellationToken cs = default)
        {
            if(string.IsNullOrEmpty(db))
                throw new ArgumentException(paramName: "db", message: "Database name, can not be null or empty");
            if(string.IsNullOrEmpty(index))
                throw new ArgumentException(paramName: "index", message: "Index, can not be null or empty");
            if(page <= 0)
                throw new ArgumentException(paramName: "page", message: "Page, must be greater than 0");
            if(pageSize <= 0)
                throw new ArgumentException(paramName: "pageSize", message: "Page size, must be greater than 0");
            var queryStringParams = new Dictionary<string, string>(){
                {"indexName", $"{index}"},
                {"compareOperator", $"{compareOperator}"},
                {"value", $"{value}"},
                {"page", $"{page}"},
                {"pageSize", $"{pageSize}"}
            };
            try{
                return await _httpClient.GetAsync(
                    endpoint: Endpoint("databases/{0}/filter"),
                    func: stream => JsonSerializer.Deserialize<MODBResponse<PagedList<string>>>(stream).Result, 
                    queryStringParams: queryStringParams,
                    cancellationToken: cs,
                    routeParams: db);
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task<bool> AnyAsync(string db, string index, CompareOperators compareOperator, string value, CancellationToken cs = default){
            if(string.IsNullOrEmpty(db))
                throw new ArgumentException(paramName: "db", message: "Database name, can not be null or empty");
            if(string.IsNullOrEmpty(index))
                throw new ArgumentException(paramName: "index", message: "Index, can not be null or empty");
            var queryStringParams = new Dictionary<string, string>(){
                {"indexName", $"{index}"},
                {"compareOperator", $"{compareOperator}"},
                {"value", $"{value}"}
            };
            try{
                return await _httpClient.GetAsync(
                    endpoint: Endpoint("databases/{0}/any"),
                    func: stream => JsonSerializer.Deserialize<MODBResponse<bool>>(stream).Result, 
                    queryStringParams: queryStringParams,
                    cancellationToken: cs,
                    routeParams: db);
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public async Task<int> CountAsync(string db, string index, CompareOperators compareOperator, string value, CancellationToken cs = default){
            if(string.IsNullOrEmpty(db))
                throw new ArgumentException(paramName: "db", message: "Database name, can not be null or empty");
            if(string.IsNullOrEmpty(index))
                throw new ArgumentException(paramName: "index", message: "Index, can not be null or empty");
            var queryStringParams = new Dictionary<string, string>(){
                {"indexName", $"{index}"},
                {"compareOperator", $"{compareOperator}"},
                {"value", $"{value}"}
            };
            try{
                return await _httpClient.GetAsync(
                    endpoint: Endpoint("databases/{0}/count"),
                    func: stream => JsonSerializer.Deserialize<MODBResponse<int>>(stream).Result, 
                    queryStringParams: queryStringParams,
                    cancellationToken: cs,
                    routeParams: db);
            }catch(HTTPRequestFailedException ex){
                throw new MODBRequestFailedException(ex, JsonSerializer.Deserialize<MODBError>(ex.Response), ex.StatusCode, ex.StatusMessage);
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}