using Blazor.FurnitureStore.Shared;

namespace Blazor.FurnitureStore.Client.Services
{
    public interface IOrderService
    {
        Task SaveOrder(Order order);

        Task<int> GetNextNumber();

        Task<IEnumerable<Order>> GetAll();

        Task<Order> GetDetails(int id);
    }
}
