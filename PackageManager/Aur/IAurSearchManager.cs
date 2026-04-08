using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PackageManager.Aur.Models;

namespace PackageManager.Aur;

public interface IAurSearchManager
{
    Task<AurResponse<AurPackageDto>> SearchAsync(string query, CancellationToken cancellationToken = default);

    Task<List<string>> SuggestAsync(string query, CancellationToken cancellationToken = default);

    Task<List<string>> SuggestByPackageBaseNamesAsync(string query,
        CancellationToken cancellationToken = default);

    Task<AurResponse<AurPackageDto>> GetInfoAsync(IEnumerable<string> packageNames,
        CancellationToken cancellationToken = default);
}