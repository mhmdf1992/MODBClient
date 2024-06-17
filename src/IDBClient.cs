using MO.MODBClient.DTOs;

namespace MO.MODBClient{
    public interface IMODBClient{
        Task<IEnumerable<string>> GetDBsAsync(CancellationToken cs = default);
        Task<DBInformation> GetDBAsync(string name, CancellationToken cs = default);
        Task CreateDBAsync(string name, CancellationToken cs = default);
        Task DeleteDBAsync(string db, CancellationToken cs = default);
        Task SetAsync(string db, string key, string type, string value, IndexItem[] indices = null, bool? createDb = true, CancellationToken cs = default);
        Task<T> GetAsync<T>(string db, string key, CancellationToken cs = default);
        Task<string> GetStringAsync(string db, string key, CancellationToken cs = default);
        Task DeleteAsync(string db, string key, CancellationToken cs = default);
        Task<PagedList<T>> GetAsync<T>(string db, string index, CompareOperators compareOperator, string value, int page = 1, int pageSize = 10, CancellationToken cs = default);
        Task<PagedList<string>> GetStringAsync(string db, string index, CompareOperators compareOperator, string value, int page = 1, int pageSize = 10, CancellationToken cs = default);
        Task<int> CountAsync(string db, string index, CompareOperators compareOperator, string value, CancellationToken cs = default);
        Task<bool> AnyAsync(string db, string index, CompareOperators compareOperator, string value, CancellationToken cs = default);
    }
}