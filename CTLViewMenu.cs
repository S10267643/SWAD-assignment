using System;
using System.Collections.Generic;
using System.Linq;

namespace SWAD_assignment
{ 
    public class CTLViewMenu
    {
        private readonly FoodStallStaff _staff;

        public CTLViewMenu(FoodStallStaff staff)
        {
            _staff = staff ?? throw new ArgumentNullException(nameof(staff));
        }

        // --------- Queries ----------
        public List<MenuItem> GetMenuForStaff()
        {
            return _staff?.Stall?.Menu?.MenuItems ?? new List<MenuItem>();
        }

        public MenuItem GetMenuItemById(int itemId)
        {
            return GetMenuForStaff().FirstOrDefault(m => m.ItemId == itemId);
        }

        // --------- Commands ----------
        public bool UpdateItem(
            int itemId,
            string newName = null,
            double? newPrice = null,
            string newDescription = null,
            int? newQuantity = null,
            bool? newAvailability = null,
            int? newPrepDelayMins = null
        )
        {
            var item = GetMenuItemById(itemId);
            if (item == null) return false;

            // Basic validation
            if (newName != null && string.IsNullOrWhiteSpace(newName)) return false;
            if (newPrice.HasValue && newPrice.Value < 0) return false;
            if (newQuantity.HasValue && newQuantity.Value < 0) return false;
            if (newPrepDelayMins.HasValue && newPrepDelayMins.Value < 0) return false;

            if (newName != null) item.ItemName = newName;
            if (newPrice.HasValue) item.Price = newPrice.Value;
            if (newDescription != null) item.ItemDescription = newDescription;
            if (newQuantity.HasValue) item.Quantity = newQuantity.Value;
            if (newAvailability.HasValue) item.IsAvailable = newAvailability.Value;
            if (newPrepDelayMins.HasValue) item.PrepDelayMinutes = newPrepDelayMins.Value;

            return true;
        }

        public bool AddItem(
            string name,
            double price,
            string description,
            int quantity,
            bool isAvailable = true,
            int prepDelayMinutes = 0
        )
        {
            var menu = _staff?.Stall?.Menu?.MenuItems;
            if (menu == null) return false;

            if (string.IsNullOrWhiteSpace(name)) return false;
            if (price < 0 || quantity < 0 || prepDelayMinutes < 0) return false;

            int nextItemId = (menu.Count == 0) ? 1 : menu.Max(m => m.ItemId) + 1;

            // Use your MenuItem ctor: (id, name, price, itemDescription, qty)
            var newItem = new MenuItem(nextItemId, name, price, description ?? string.Empty, quantity)
            {
                IsAvailable = isAvailable,
                PrepDelayMinutes = prepDelayMinutes
            };

            menu.Add(newItem);
            return true;
        }

        public bool DeleteItem(int itemId)
        {
            var menu = _staff?.Stall?.Menu?.MenuItems;
            if (menu == null) return false;

            var item = menu.FirstOrDefault(m => m.ItemId == itemId);
            if (item == null) return false;

            menu.Remove(item);
            return true;
        }
    }
}
