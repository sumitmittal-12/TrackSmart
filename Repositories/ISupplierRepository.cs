using TrackSmart.DTOs;
using TrackSmart.Models;

namespace TrackSmart.Repositories
{
    public interface ISupplierRepository
    {
        Task<List<SupplierDto>> GetSuppliersWithItemsAsync(string retailerId);
        Task<CreateSupplierDto?> GetSupplierDtoForEditAsync(int supplierId, string retailerId);
        Task<Supplier?> GetSupplierByNameAsync(string companyName, string retailerId);
        Task<Supplier?> GetSupplierByIdAsync(int id, string retailerId);
        Task AddSupplierAsync(Supplier supplier);
        void RemoveItemSuppliers(IEnumerable<ItemSupplier> itemSuppliers);
        void AddItemSuppliers(IEnumerable<ItemSupplier> itemSuppliers);
        Task SaveChangesAsync();
    }
}