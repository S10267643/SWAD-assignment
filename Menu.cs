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
    }

}
