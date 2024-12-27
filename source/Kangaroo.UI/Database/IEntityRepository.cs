using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kangaroo.UI.Database;

/// <summary>
/// Reads and writes entities in a database
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TUniqueId"></typeparam>
public interface IEntityRepository<TEntity, in TUniqueId> 
    where TEntity : notnull 
    where TUniqueId: notnull
{
    /// <summary>
    /// Returns all the entities in the repository
    /// <param name="token">cancellation token with default assignment</param>
    /// </summary>
    /// <returns>User accounts</returns>
    Task<IEnumerable<TEntity>> GetAsync(CancellationToken token = default);

    /// <summary>
    /// Returns the entity with the specified ID
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="token">cancellation token with default assignment</param>
    /// <returns>the nullable entity</returns>
    Task<TEntity?> GetByIdAsync(TUniqueId id, CancellationToken token = default);

    /// <summary>
    /// Creates a new entity in the repository.
    /// <remarks>Will return false if the ID or account name already exists</remarks>
    /// </summary>
    /// <param name="entity">New User Account to Add</param>
    /// <param name="token">cancellation token with default assignment</param>
    /// <returns>Results</returns>
    Task<bool> CreateAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    /// Updates the entity in the repository.
    /// <remarks>Will return false if the ID or entity exists</remarks>
    /// </summary>
    /// <param name="entity">New User Account to Add</param>
    /// <param name="token">cancellation token with default assignment</param>
    /// <returns>Results</returns>
    Task<bool> UpdateAsync(TEntity entity, CancellationToken token = default);

    /// <summary>
    /// Deletes the specified entity with the ID
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="token"></param>
    /// <returns>Result</returns>
    Task<bool> DeleteByIdAsync(TUniqueId id, CancellationToken token = default);

    /// <summary>
    /// Determines if the Entity exists in the repository.
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="token"></param>
    /// <returns>True if account is valid</returns>
    Task<bool> ExistsById(TUniqueId id, CancellationToken token = default);
}