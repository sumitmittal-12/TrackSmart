using TrackSmart.DTOs;
using TrackSmart.Models;

namespace TrackSmart.Repositories
{
    public interface ISupplierRepository
    {
        // --- READS (Dapper - returns DTOs for the UI) ---
        Task<List<SupplierDto>> GetSuppliersWithItemsAsync(string retailerId);
        Task<CreateSupplierDto?> GetSupplierDtoForEditAsync(int supplierId, string retailerId);

        // --- READS (EF Core - returns tracked Entities for Business Logic) ---
        Task<Supplier?> GetSupplierByNameAsync(string companyName, string retailerId);
        Task<Supplier?> GetSupplierByIdAsync(int id, string retailerId);

        // --- WRITES (EF Core) ---
        Task AddSupplierAsync(Supplier supplier);
        void RemoveItemSuppliers(IEnumerable<ItemSupplier> itemSuppliers);
        void AddItemSuppliers(IEnumerable<ItemSupplier> itemSuppliers);
        Task SaveChangesAsync();
    }
}