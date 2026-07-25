using ClosedXML.Excel;
using TrackSmart.DTOs;
using TrackSmart.Models;
using TrackSmart.Repositories;

namespace TrackSmart.Services
{
    public class ItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        // READ OPERATIONS

        public async Task<List<ItemDto>> GetItemsAsync(string retailerId)
            => await _itemRepository.GetItemsAsync(retailerId);

        public async Task<List<ItemDto>> GetItemsWithSuppliersAsync(string retailerId)
            => await _itemRepository.GetItemsWithSuppliersAsync(retailerId);

        public async Task<ItemDto?> GetItemByIdAsync(int id, string retailerId)
            => await _itemRepository.GetItemDtoByIdAsync(id, retailerId);


        // WRITE OPERATIONS

        public async Task DeleteItemAsync(int id, string retailerId)
        {
            var item = await _itemRepository.GetItemEntityByIdAsync(id, retailerId);
            if (item != null)
            {
                item.isActive = false;
                await _itemRepository.SaveChangesAsync();
            }
        }

        public async Task CreateItemAsync(CreateItemDto dto, string retailerId)
        {
            var existingItem = await _itemRepository.GetItemByNameAndCompanyAsync(dto.Name, dto.CompanyName, retailerId);

            if (existingItem != null)
            {
                if (existingItem.isActive)
                {
                    throw new InvalidOperationException($"An active item named '{dto.Name}' by '{dto.CompanyName}' already exists in your inventory.");
                }
                else
                {
                    // Resurrecting soft-deleted item
                    existingItem.isActive = true;
                    existingItem.Description = dto.Description ?? string.Empty;
                    existingItem.OriginalPrice = dto.OriginalPrice ?? 0m;
                    existingItem.DiscountPercentage = dto.DiscountPercentage ?? 0m;
                    existingItem.StockCount = dto.StockCount ?? 0;
                    existingItem.LowStockThreshold = dto.LowStockThreshold ?? 0;

                    await _itemRepository.SaveChangesAsync();
                    return;
                }
            }

            // Creating new item
            var newItem = new Item
            {
                Name = dto.Name,
                CompanyName = dto.CompanyName,
                Description = dto.Description ?? string.Empty,
                OriginalPrice = dto.OriginalPrice ?? 0m,
                DiscountPercentage = dto.DiscountPercentage ?? 0m,
                StockCount = dto.StockCount ?? 0,
                LowStockThreshold = dto.LowStockThreshold ?? 0,
                RetailerId = retailerId,
                isActive = true
            };

            await _itemRepository.AddItemAsync(newItem);
            await _itemRepository.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(int id, CreateItemDto dto, string retailerId)
        {
            var item = await _itemRepository.GetItemEntityByIdAsync(id, retailerId);

            if (item != null)
            {
                item.Name = dto.Name;
                item.CompanyName = dto.CompanyName;
                item.Description = dto.Description ?? string.Empty;
                item.OriginalPrice = dto.OriginalPrice ?? 0m;
                item.DiscountPercentage = dto.DiscountPercentage ?? 0m;
                item.StockCount = dto.StockCount ?? 0;
                item.LowStockThreshold = dto.LowStockThreshold ?? 0;

                await _itemRepository.SaveChangesAsync();
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have permission to modify this item.");
            }
        }

        public async Task<int> ImportItemsFromExcelAsync(Stream fileStream, string retailerId)
        {
            var itemsToInsert = new List<Item>();

            using (var workbook = new XLWorkbook(fileStream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    var name = row.Cell(1).GetString();
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    var companyName = row.Cell(2).GetString();
                    var description = row.Cell(3).GetString();

                    decimal.TryParse(row.Cell(4).GetString(), out decimal price);
                    decimal.TryParse(row.Cell(5).GetString(), out decimal discount);
                    int.TryParse(row.Cell(6).GetString(), out int stock);
                    int.TryParse(row.Cell(7).GetString(), out int threshold);

                    itemsToInsert.Add(new Item
                    {
                        Name = name,
                        CompanyName = companyName,
                        Description = description,
                        OriginalPrice = price,
                        DiscountPercentage = discount,
                        StockCount = stock,
                        LowStockThreshold = threshold,
                        RetailerId = retailerId,
                        isActive = true
                    });
                }
            }

            if (itemsToInsert.Any())
            {
                await _itemRepository.AddItemsBulkAsync(itemsToInsert);
                await _itemRepository.SaveChangesAsync();
            }

            return itemsToInsert.Count;
        }
    }
}