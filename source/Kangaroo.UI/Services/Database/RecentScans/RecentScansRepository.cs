using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kangaroo.UI.Services.Database;

public sealed class RecentScansRepository : IEntityRepository<RecentScan, Guid>
{
    /// <inheritdoc />
    public async Task<IEnumerable<RecentScan>> GetAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<RecentScan?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<bool> CreateAsync(RecentScan entity, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(RecentScan entity, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsById(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
