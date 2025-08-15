using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Menu
    {
        public List<MenuItem> MenuItems { get; set; }
        public int MenuId { get; set; }

        public Menu()
        {
            MenuItems = new List<MenuItem>();
        }

        public Menu(int menuId)
        {
            MenuId = menuId;
            MenuItems = new List<MenuItem>();
        }

        public void AddMenuItem(MenuItem item)
        {
            MenuItems.Add(item);
        }

        public void RemoveMenuItem(int itemId)
        {
            MenuItems.RemoveAll(i => i.ItemId == itemId);
        }

        public void DisplayMenu()
        {
            foreach (var item in MenuItems)
            {
                Console.WriteLine(item);
            }
        }

        public MenuItem GetMenuItemById(int itemId)
        {
            return MenuItems.Find(i => i.ItemId == itemId);
        }

        public List<MenuItem> GetAllItems() => MenuItems;
    }
}
