using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TrackSmart.Data;
using TrackSmart.DTOs;
using TrackSmart.Models;

namespace TrackSmart.Services
{
    public class ItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnectionFactory _connectionFactory;

        // Inject BOTH contexts: EF Core for commands (writes), Factory for queries (reads)
        public ItemService(ApplicationDbContext context, IDbConnectionFactory connectionFactory)
        {
            _context = context;
            _connectionFactory = connectionFactory;
        }

        // --- READ OPERATIONS (Powered by Dapper) ---

        public async Task<List<ItemDto>> GetItemsAsync(string retailerId)
        {
            // Added AND IsActive = 1 (In SQL Server, bit fields are 1 for true, 0 for false)
            const string sql = @"
            SELECT 
                Id, 
                Name, 
                CompanyName,
                OriginalPrice, 
                DiscountPercentage, 
                StockCount, 
                LowStockThreshold
            FROM Items 
            WHERE RetailerId = @RetailerId AND isActive = 1
            ORDER BY Name ASC";

            using (var connection = _connectionFactory.CreateConnection())
            {
                var items = await connection.QueryAsync<ItemDto>(sql, new { RetailerId = retailerId });
                return items.ToList();
            }
        }

        public async Task<ItemDto?> GetItemByIdAsync(int id, string retailerId)
        {
            const string sql = @"
            SELECT 
                Id, 
                Name, 
                CompanyName,
                OriginalPrice, 
                DiscountPercentage, 
                StockCount, 
                LowStockThreshold
            FROM Items 
            WHERE Id = @Id AND RetailerId = @RetailerId AND isActive = 1";

            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<ItemDto>(sql, new { Id = id, RetailerId = retailerId });
            }
        }


        // --- WRITE OPERATIONS (Powered by EF Core) ---

        public async Task DeleteItemAsync(int id, string retailerId)
        {
            //var item = await _context.Items
            //.FirstOrDefaultAsync(i => i.Id == id && i.RetailerId == retailerId);

            //if (item != null)
            //{
            //    // SOFT DELETE: Instead of _context.Items.Remove(item), we just flip the flag
            //    item.isActive = false;

            //    await _context.SaveChangesAsync();
            //}

            var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == id && i.RetailerId == retailerId);

            if (item != null)
            {
                // 2. The Hard Delete Command
                _context.Items.Remove(item);

                // 3. Commit the transaction to SQL Server
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new UnauthorizedAccessException("Item not found or you do not have permission to delete it.");
            }
        }

        public async Task CreateItemAsync(CreateItemDto dto, string retailerId)
        {
            bool itemExists = await _context.Items
            .AnyAsync(i => i.RetailerId == retailerId
                     && i.Name.ToLower() == dto.Name.ToLower()
                     && i.CompanyName.ToLower() == dto.CompanyName.ToLower()
                     && i.isActive);

            if (itemExists)
            {
                // 2. Throw a domain exception to halt the transaction
                throw new InvalidOperationException($"An active item named '{dto.Name}' already exists in your inventory.");
            }

            var item = new Item
            {
                Name = dto.Name,
                CompanyName = dto.CompanyName,
                OriginalPrice = dto.OriginalPrice ?? 0m,
                DiscountPercentage = dto.DiscountPercentage ?? 0m,
                StockCount = dto.StockCount ?? 0,
                LowStockThreshold = dto.LowStockThreshold ?? 0,
                RetailerId = retailerId,
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(int id, CreateItemDto dto, string retailerId)
        {
            // Find the item AND ensure the RetailerId matches
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.RetailerId == retailerId);

            if (item != null)
            {
                // Apply the updated values
                item.Name = dto.Name;
                item.CompanyName = dto.CompanyName;
                item.OriginalPrice = dto.OriginalPrice ?? 0m;
                item.DiscountPercentage = dto.DiscountPercentage ?? 0m;
                item.StockCount = dto.StockCount ?? 0;
                item.LowStockThreshold = dto.LowStockThreshold ?? 0;

                // Save changes to SQL Server
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have permission to modify this item.");
            }
        }
    }
}