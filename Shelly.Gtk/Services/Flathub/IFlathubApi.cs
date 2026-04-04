namespace Shelly.Gtk.Services.FlatHub;

public interface IFlatHubApiService
{
   public Task GetStatsForAppAsync(string appId);
    
   public Task<List<string>> GetCollectionTrendingAsync(int page = 1, int perPage = 20);
   
   public Task<List<string>> GetCollectionPopularAsync(int page = 1, int perPage = 20);
   
   public Task<List<string>> GetCollectionRecentlyUpdatedAsync(int page = 1, int perPage = 20);
   
   public Task<List<string>> GetCollectionRecentlyAddedAsync(int page = 1, int perPage = 20);
    
}