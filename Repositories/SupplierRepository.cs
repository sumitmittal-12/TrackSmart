using Dapper;
using Microsoft.EntityFrameworkCore;
using TrackSmart.Data;
using TrackSmart.DTOs;
using TrackSmart.Models;

namespace TrackSmart.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnectionFactory _connectionFactory;

        public SupplierRepository(ApplicationDbContext context, IDbConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        public async Task<List<SupplierDto>> GetSuppliersWithItemsAsync(string retailerId)
        {
            const string sql = @"
                SELECT 
                    s.Id, s.CompanyName, s.ContactEmail, s.ContactPhone, s.AddressLine, s.City, s.State, s.PostalCode,
                    -- Dapper splits here
                    i.Id, i.Name, i.CompanyName, i.Description, i.OriginalPrice, i.DiscountPercentage, i.StockCount, i.LowStockThreshold
                FROM Suppliers s
                LEFT JOIN ItemSuppliers isup ON s.Id = isup.SupplierId
                LEFT JOIN Items i ON isup.ItemId = i.Id AND i.isActive = 1
                WHERE s.RetailerId = @RetailerId AND s.isActive=1
                ORDER BY s.CompanyName ASC";

            using var connection = _connectionFactory.CreateConnection();
            var supplierDict = new Dictionary<int, SupplierDto>();

            await connection.QueryAsync<SupplierDto, ItemDto, SupplierDto>(
                sql,
                (supplier, item) =>
                {
                    if (!supplierDict.TryGetValue(supplier.Id, out var currentSupplier))
                    {
                        currentSupplier = supplier;
                        currentSupplier.SuppliedItems = new List<ItemDto>();
                        supplierDict.Add(currentSupplier.Id, currentSupplier);
                    }

                    if (item != null && item.Id > 0 && !currentSupplier.SuppliedItems.Any(x => x.Id == item.Id))
                    {
                        currentSupplier.SuppliedItems.Add(item);
                    }

                    return currentSupplier;
                },
                new { RetailerId = retailerId },
                splitOn: "Id"
            );

            return supplierDict.Values.ToList();
        }

        public async Task<CreateSupplierDto?> GetSupplierDtoForEditAsync(int supplierId, string retailerId)
        {
            const string sql = @"
                SELECT CompanyName, ContactEmail, ContactPhone, AddressLine, City, State, PostalCode 
                FROM Suppliers 
                WHERE Id = @Id AND RetailerId = @RetailerId AND IsActive = 1;

                SELECT ItemId 
                FROM ItemSuppliers 
                WHERE SupplierId = @Id;";

            using var connection = _connectionFactory.CreateConnection();

            // multi object acts as a cursor that allows you to read those result sets one by one in the exact order you wrote them in the SQL string.
            using var multi = await connection.QueryMultipleAsync(sql, new { Id = supplierId, RetailerId = retailerId });

            var supplierDto = await multi.ReadSingleOrDefaultAsync<CreateSupplierDto>();
            if (supplierDto == null) return null;

            var itemIds = await multi.ReadAsync<int>();
            supplierDto.ItemIds = itemIds.ToList();

            return supplierDto;
        }

        public async Task<Supplier?> GetSupplierByNameAsync(string companyName, string retailerId)
        {
            return await _context.Suppliers
                .Include(s => s.ItemSuppliers)
                .FirstOrDefaultAsync(s => s.CompanyName == companyName && s.RetailerId == retailerId);
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id, string retailerId)
        {
            return await _context.Suppliers
                .Include(s => s.ItemSuppliers)
                .FirstOrDefaultAsync(s => s.Id == id && s.RetailerId == retailerId);
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
        }

        public void RemoveItemSuppliers(IEnumerable<ItemSupplier> itemSuppliers)
        {
            _context.ItemSuppliers.RemoveRange(itemSuppliers);
        }

        public void AddItemSuppliers(IEnumerable<ItemSupplier> itemSuppliers)
        {
            _context.ItemSuppliers.AddRange(itemSuppliers);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}