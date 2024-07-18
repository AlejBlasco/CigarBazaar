namespace CigarBazaar.Domain.Contracts
{
    public interface IRepository<T> where T : class
    {
        Task CreateItemAsync(T item, CancellationToken cancellationToken = default);

        Task UpdateItemAsync(T item, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllItemsAsync(CancellationToken cancellationToken = default);
    }
}
