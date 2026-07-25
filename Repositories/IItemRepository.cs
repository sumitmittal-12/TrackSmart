using TrackSmart.DTOs;
using TrackSmart.Models;

namespace TrackSmart.Repositories
{
    public interface IItemRepository
    {
        Task<List<ItemDto>> GetItemsAsync(string retailerId);
        Task<List<ItemDto>> GetItemsWithSuppliersAsync(string retailerId);
        Task<ItemDto?> GetItemDtoByIdAsync(int id, string retailerId);
        Task<Item?> GetItemEntityByIdAsync(int id, string retailerId);
        Task<Item?> GetItemByNameAndCompanyAsync(string name, string companyName, string retailerId);
        Task AddItemAsync(Item item);
        Task AddItemsBulkAsync(IEnumerable<Item> items);
        Task SaveChangesAsync();
    }
}