namespace Blazor.FurnitureStore.Client.Services
{
    public interface IClientService
    {
        Task<IEnumerable<Blazor.FurnitureStore.Shared.Client>> GetAll();
    }
}
