using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class CTLPlaceOrder
    {
        private Menu _menu;
        private Cart _cart;

        public CTLPlaceOrder(Menu menu, Student student)
        {
            _menu = menu;
            _cart = new Cart(student);
        }

        public bool AddToCart(int itemId, int quantity)
        {
            MenuItem item = _menu.GetMenuItemById(itemId);
            if (item == null || !item.IsAvailable || item.Quantity < quantity)
                return false;

            _cart.AddToCart(item, quantity);
            item.ReduceStock(quantity);
            return true;
        }

        public void UpdateCartItem(int itemId, int newQuantity)
        {
            var existingItem = _cart.GetCartItems().FirstOrDefault(ci => ci.Item.ItemId == itemId);
            if (existingItem.Item != null)
            {
                int difference = existingItem.Quantity - newQuantity;
                if (difference > 0)
                {
                    existingItem.Item.IncreaseStock(difference);
                }
                else if (difference < 0)
                {
                    existingItem.Item.ReduceStock(-difference);
                }
                _cart.UpdateQuantity(itemId, newQuantity);
            }
        }

        public Order Checkout(FoodStall stall)
        {
            var order = new Order(_cart.GetCartItems(), _cart.CalculateTotalPrice(), _cart.Owner);
            order.GenerateQRCode();
            order.NotifyStall(stall);
            _cart.EmptyCart();
            return order;
        }

        public Cart GetCart() => _cart;
        public Menu GetMenu() => _menu;
    }
}
