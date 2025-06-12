// Repositories/IRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace HealthcareGraphAPI.Repositories
{
    /// <summary>
    /// Generic repository interface providing basic CRUD operations.
    /// </summary>
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<TResult>> GetAllSourcesByCriteriaAsync<TResult>(
        string relationship,
        string targetNodeLabel,
        string targetId,
        Func<INode, TResult> mapper);

        Task<IEnumerable<TResult>> GetAllTargetsByCriteriaAsync<TResult>(
        string relationship,
        string targetNodeLabel,
        string targetId,
        Func<INode, TResult> mapper);

        Task<T> GetByIdAsync(string id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);

        Task AssignRelationshipAsync<TSource, TTarget>(
          string sourceId,
          string targetId,
          string relationshipType);
    }
}
