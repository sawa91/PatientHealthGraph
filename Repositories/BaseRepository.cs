// Repositories/BaseRepository.cs
using Neo4j.Driver;
using System;
using HealthcareGraphAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthcareGraphAPI.Repositories
{
    /// <summary>
    /// Base repository for managing common Neo4j CRUD operations for entities implementing IDomainEntity.
    /// </summary>
    public abstract class BaseRepository<T> : IRepository<T> where T : HealthcareGraphAPI.Models.IEntity
    {
        protected readonly IDriver _driver;

        /// <summary>
        /// The label used in Neo4j for the entity (e.g., "Patient").
        /// </summary>
        protected abstract string NodeLabel { get; }

        /// <summary>
        /// Maps a Neo4j INode to a domain entity instance.
        /// </summary>
        protected abstract T Map(INode node);

        /// <summary>
        /// Creates a dictionary of parameters from the given entity.
        /// </summary>
        protected abstract IDictionary<string, object> CreateParameters(T entity);

        public BaseRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var results = new List<T>();
            var query = $"MATCH (n:{NodeLabel}) WHERE n.active = true RETURN n";
            var session = _driver.AsyncSession();
            try
            {
                var cursor = await session.RunAsync(query);
                while (await cursor.FetchAsync())
                {
                    var node = cursor.Current["n"].As<INode>();
                    results.Add(Map(node));
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return results;
        }
        /* 
              GetAllSourcesByCriteriaAsync method Retrieves all **source nodes** connected to the given **target node**.
               Example: (source)-[:RELATION]->(t {id: targetId})
               - Finds entities pointing toward the target.
               - Only returns active nodes.
            */
        public async Task<IEnumerable<TResult>> GetAllSourcesByCriteriaAsync<TResult>(
        string relationship,
        string targetNodeLabel,
        string targetId,
        Func<INode, TResult> mapper)
        {
            var results = new List<TResult>();

            var query = $@"
        
        MATCH (s)-[:{relationship}]->(t:{targetNodeLabel} {{id: $targetId}})
        WHERE s.active = true
        RETURN s";

            Console.WriteLine($"Record query: {query}");

            await using var session = _driver.AsyncSession();
            try
            {
                var cursor = await session.RunAsync(query, new { targetId });
                while (await cursor.FetchAsync())
                {
                    var node = cursor.Current["s"].As<INode>();
                    Console.WriteLine($"Node properties: {node}");
                    results.Add(mapper(node)); // 
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            Console.WriteLine($"results : {results}");

            return results;
        }

        /* 
                   GetAllTargetsByCriteriaAsync method Retrieves all target nodes linked from the given **source node**.
                   Example: (source {id: sourceId})-[:RELATION]->(target)
                   - Finds entities pointed to by the source.
                   - Only returns active nodes.
                */
        public async Task<IEnumerable<TResult>> GetAllTargetsByCriteriaAsync<TResult>(
                string relationship,
                string sourceNodeLabel,
                string sourceId,
                Func<INode, TResult> mapper)
        {
            var results = new List<TResult>();

            var query = $@"
        
        MATCH (s:{sourceNodeLabel} {{id: $sourceId}})-[:{relationship}]->(t)
        WHERE t.active = true
        RETURN t";

            await using var session = _driver.AsyncSession();
            try
            {
                var cursor = await session.RunAsync(query, new { sourceId });
                while (await cursor.FetchAsync())
                {
                    var node = cursor.Current["t"].As<INode>();
                    results.Add(mapper(node)); // 
                }
            }
            finally
            {
                await session.CloseAsync();
            }

            return results;
        }



        public async Task<T> GetByIdAsync(string id)
        {
            var query = $"MATCH (n:{NodeLabel} {{id: $id}}) RETURN n";
            var session = _driver.AsyncSession();
            T entity = default;
            try
            {
                var cursor = await session.RunAsync(query, new { id });
                if (await cursor.FetchAsync())
                {
                    var node = cursor.Current["n"].As<INode>();
                    entity = Map(node);
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return entity;
        }

        public async Task CreateAsync(T entity)
        {
            var query = $"CREATE (n:{NodeLabel} $props) RETURN n";
            var parameters = new { props = CreateParameters(entity) };
            var session = _driver.AsyncSession();
            try
            {
                await session.RunAsync(query, parameters);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            // Build a dictionary of entity's properties.
            var updateProps = CreateParameters(entity);
            // Remove the "id" property so it is not updated.
            updateProps.Remove("id");

            // Remove "updatedAt" if it exists in the dictionary,
            // since we will set it separately.
            if (updateProps.ContainsKey("updatedAt"))
                updateProps.Remove("updatedAt");

            // Set updatedAt to the current time.
            DateTime updatedAt = DateTime.UtcNow;

            // Query: match the node by id, merge the new properties, 
            // and explicitly set updatedAt by converting the parameter using datetime().
            var query = $"MATCH (n:{NodeLabel} {{ id: $id }}) " +
                        "SET n += $props, " +
                        "n.updatedAt = datetime($updatedAt)";

            await using var session = _driver.AsyncSession();
            await session.RunAsync(query, new
            {
                id = entity.Id,
                props = updateProps,
                updatedAt = updatedAt.ToUniversalTime().ToString("o") // ISO 8601 formatted string
            });
        }


        public async Task DeleteAsync(string id)
        {
            var query = $"MATCH (n:{NodeLabel} {{ id: $id }}) set n.active=false";
            await using var session = _driver.AsyncSession();
            await session.RunAsync(query, new { id });
        }

        public async Task AssignRelationshipAsync<TSource, TTarget>(
            string sourceId, 
            string targetId, 
            string relationshipType)
        {
            string sourceLabel = typeof(TSource).Name;
            string targetLabel = typeof(TTarget).Name;
 
            // Prepare the parameterized Cypher query
            string query = $@"
                MATCH (source:{sourceLabel} {{ id: $sourceId }})
                MATCH (target:{targetLabel} {{ id: $targetId }})
                MERGE (source)-[r:{relationshipType}]->(target)
                RETURN r";
            var parameters = new { sourceId, targetId };

            // Use ExecuteWriteAsync to run the query inside a write transaction.
            await using var session = _driver.AsyncSession();
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, parameters);
            });
        }


            }
        }
