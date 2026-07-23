using TrackSmart.DTOs;
using TrackSmart.Models;
using TrackSmart.Repositories;

namespace TrackSmart.Services
{
    public class SupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        // --- READS ---

        public async Task<List<SupplierDto>> GetSuppliersWithItemsAsync(string retailerId)
        {
            return await _supplierRepository.GetSuppliersWithItemsAsync(retailerId);
        }

        public async Task<CreateSupplierDto?> GetSupplierForEditAsync(int supplierId, string retailerId)
        {
            return await _supplierRepository.GetSupplierDtoForEditAsync(supplierId, retailerId);
        }

        // --- WRITES ---

        public async Task CreateSupplierAsync(CreateSupplierDto dto, string retailerId)
        {
            var existingSupplier = await _supplierRepository.GetSupplierByNameAsync(dto.CompanyName, retailerId);

            if (existingSupplier != null)
            {
                if (existingSupplier.isActive)
                {
                    throw new InvalidOperationException("A supplier with this company name already exists.");
                }
                else
                {
                    // Soft-deleted logic
                    existingSupplier.isActive = true;
                    existingSupplier.ContactEmail = dto.ContactEmail;
                    existingSupplier.ContactPhone = dto.ContactPhone;
                    existingSupplier.AddressLine = dto.AddressLine ?? string.Empty;
                    existingSupplier.City = dto.City ?? string.Empty;
                    existingSupplier.State = dto.State ?? string.Empty;
                    existingSupplier.PostalCode = dto.PostalCode ?? string.Empty;

                    _supplierRepository.RemoveItemSuppliers(existingSupplier.ItemSuppliers);

                    if (dto.ItemIds != null && dto.ItemIds.Any())
                    {
                        var newMappings = dto.ItemIds.Select(itemId => new ItemSupplier
                        {
                            ItemId = itemId,
                            SupplierId = existingSupplier.Id
                        });
                        _supplierRepository.AddItemSuppliers(newMappings);
                    }

                    await _supplierRepository.SaveChangesAsync();
                    return;
                }
            }

            var newSupplier = new Supplier
            {
                CompanyName = dto.CompanyName,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                AddressLine = dto.AddressLine ?? string.Empty,
                City = dto.City ?? string.Empty,
                State = dto.State ?? string.Empty,
                PostalCode = dto.PostalCode ?? string.Empty,
                RetailerId = retailerId,
                isActive = true
            };

            await _supplierRepository.AddSupplierAsync(newSupplier);
            await _supplierRepository.SaveChangesAsync();

            if (dto.ItemIds != null && dto.ItemIds.Any())
            {
                var newMappings = dto.ItemIds.Select(itemId => new ItemSupplier
                {
                    ItemId = itemId,
                    SupplierId = newSupplier.Id
                });
                _supplierRepository.AddItemSuppliers(newMappings);
                await _supplierRepository.SaveChangesAsync();
            }
        }

        public async Task UpdateSupplierAsync(int supplierId, CreateSupplierDto dto, string retailerId)
        {
            var supplier = await _supplierRepository.GetSupplierByIdAsync(supplierId, retailerId);

            if (supplier == null) throw new Exception("Supplier not found or access denied.");

            supplier.CompanyName = dto.CompanyName;
            supplier.ContactEmail = dto.ContactEmail;
            supplier.ContactPhone = dto.ContactPhone;
            supplier.AddressLine = dto.AddressLine;
            supplier.City = dto.City;
            supplier.State = dto.State;
            supplier.PostalCode = dto.PostalCode;

            var existingItemIds = supplier.ItemSuppliers.Select(x => x.ItemId).ToList();
            var newItemIds = dto.ItemIds ?? new List<int>();

            var toRemove = supplier.ItemSuppliers.Where(x => !newItemIds.Contains(x.ItemId)).ToList();
            var toAdd = newItemIds.Where(id => !existingItemIds.Contains(id))
                                  .Select(id => new ItemSupplier { ItemId = id, SupplierId = supplier.Id })
                                  .ToList();

            _supplierRepository.RemoveItemSuppliers(toRemove);
            _supplierRepository.AddItemSuppliers(toAdd);

            await _supplierRepository.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int supplierId, string retailerId)
        {
            var supplier = await _supplierRepository.GetSupplierByIdAsync(supplierId, retailerId);

            if (supplier != null)
            {
                // Soft delete using EF Core
                supplier.isActive = false;
                await _supplierRepository.SaveChangesAsync();
            }
        }
    }
}