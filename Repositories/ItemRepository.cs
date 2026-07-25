using Dapper;
using Microsoft.EntityFrameworkCore;
using TrackSmart.Data;
using TrackSmart.DTOs;
using TrackSmart.Models;

namespace TrackSmart.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnectionFactory _connectionFactory;

        public ItemRepository(ApplicationDbContext context, IDbConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        // DAPPER READS
        public async Task<List<ItemDto>> GetItemsAsync(string retailerId)
        {
            const string sql = @"
                SELECT Id, Name, CompanyName, Description, OriginalPrice, DiscountPercentage, StockCount, LowStockThreshold
                FROM Items 
                WHERE RetailerId = @RetailerId AND isActive = 1
                ORDER BY Name ASC";

            using var connection = _connectionFactory.CreateConnection();
            var items = await connection.QueryAsync<ItemDto>(sql, new { RetailerId = retailerId });
            return items.ToList();
        }

        public async Task<List<ItemDto>> GetItemsWithSuppliersAsync(string retailerId)
        {
            const string sql = @"
                SELECT 
                    i.Id, i.Name, i.CompanyName, i.Description, i.OriginalPrice, i.DiscountPercentage, i.StockCount, i.LowStockThreshold,
                    -- Dapper splits here
                    s.Id, s.CompanyName, s.ContactEmail, s.ContactPhone, s.AddressLine, s.City, s.State, s.PostalCode
                FROM Items i
                LEFT JOIN ItemSuppliers isup ON i.Id = isup.ItemId
                LEFT JOIN Suppliers s ON isup.SupplierId = s.Id AND s.isActive=1
                WHERE i.RetailerId = @RetailerId AND i.isActive = 1
                ORDER BY i.Name ASC";

            using var connection = _connectionFactory.CreateConnection();
            var itemDictionary = new Dictionary<int, ItemDto>();

            await connection.QueryAsync<ItemDto, SupplierDto, ItemDto>(
                sql,
                (item, supplier) =>
                {
                    if (!itemDictionary.TryGetValue(item.Id, out var currentItem))
                    {
                        currentItem = item;
                        currentItem.Suppliers = new List<SupplierDto>();
                        itemDictionary.Add(currentItem.Id, currentItem);
                    }

                    if (supplier != null && supplier.Id > 0 && !currentItem.Suppliers.Any(s => s.Id == supplier.Id))
                    {
                        currentItem.Suppliers.Add(supplier);
                    }

                    return currentItem;
                },
                new { RetailerId = retailerId },
                splitOn: "Id"
            );

            return itemDictionary.Values.ToList();
        }

        public async Task<ItemDto?> GetItemDtoByIdAsync(int id, string retailerId)
        {
            const string sql = @"
                SELECT Id, Name, CompanyName, Description, OriginalPrice, DiscountPercentage, StockCount, LowStockThreshold
                FROM Items 
                WHERE Id = @Id AND RetailerId = @RetailerId AND isActive = 1";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<ItemDto>(sql, new { Id = id, RetailerId = retailerId });
        }

        public async Task<Item?> GetItemEntityByIdAsync(int id, string retailerId)
        {
            // Using EF Core so it tracks updates and deletes automatically!
            return await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.RetailerId == retailerId);
        }

        public async Task<Item?> GetItemByNameAndCompanyAsync(string name, string companyName, string retailerId)
        {
            return await _context.Items
                .FirstOrDefaultAsync(i =>
                    i.RetailerId == retailerId &&
                    i.Name.ToLower() == name.ToLower() &&
                    i.CompanyName.ToLower() == companyName.ToLower());
        }

        //EF CORE WRITES
        public async Task AddItemAsync(Item item)
        {
            await _context.Items.AddAsync(item);
        }

        public async Task AddItemsBulkAsync(IEnumerable<Item> items)
        {
            await _context.Items.AddRangeAsync(items);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}