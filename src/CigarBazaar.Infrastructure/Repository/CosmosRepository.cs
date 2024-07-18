using CigarBazaar.Domain.Contracts;
using CigarBazaar.Shared.Models;
using Microsoft.Azure.Cosmos;

namespace CigarBazaar.Infrastructure.Repository;

public class CosmosRepository<T> where T : Entity
{
    private readonly Container container;

    private const string GET_ALL_QUERY = "SELECT * FROM c";

    public CosmosRepository(string accountEndpoint, string authKey, string databaseId, string containerId)
    {
        if (string.IsNullOrWhiteSpace(accountEndpoint))
            throw new ArgumentNullException(nameof(accountEndpoint));
        if (string.IsNullOrWhiteSpace(authKey))
            throw new ArgumentNullException(nameof(authKey));
        if (string.IsNullOrWhiteSpace(databaseId))
            throw new ArgumentNullException(nameof(databaseId));
        if (string.IsNullOrWhiteSpace(containerId))
            throw new ArgumentNullException(nameof(containerId));

        CosmosClient client = new CosmosClient(accountEndpoint: accountEndpoint,
             authKeyOrResourceToken: authKey);

        Database database = client.GetDatabase(databaseId);

        container = client.GetContainer(databaseId: database.Id,
            containerId: containerId);
    }

    public async Task CreateItemAsync(T item, CancellationToken cancellationToken = default)
    {
        var response = await container.CreateItemAsync(item: item,
            partitionKey: new PartitionKey(item.Id),
            cancellationToken: cancellationToken);
    }

    public async Task UpsertItemAsync(T item, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await container.UpsertItemAsync(item: item,
            partitionKey: new PartitionKey(item.Name),
            cancellationToken: cancellationToken);
        }
        catch(Exception ex)
        {
            var serr = ex.Message;
        }
    }

    public async Task<IEnumerable<T>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        QueryDefinition queryDefinition = new QueryDefinition(GET_ALL_QUERY);
        FeedIterator<T> queryResultSetIterator = container.GetItemQueryIterator<T>(queryDefinition);

        List<T> items = new List<T>();
        while (queryResultSetIterator.HasMoreResults)
        {
            FeedResponse<T> currentResultSet = await queryResultSetIterator.ReadNextAsync();
            foreach (T item in currentResultSet)
            {
                items.Add(item);
            }
        }

        return items;
    }

}
