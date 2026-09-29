using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFbowlingKurs.AppData
{
    public class ProductCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVisible { get; set; }

        public string PhotoPath
        {
            get { return PhotoHelper.Find("products", ImageUrl); }
        }

        public string StockText
        {
            get
            {
                if (!IsVisible) return "Скрыт с сайта";
                if (Quantity == 0) return "Нет в наличии";
                return "В наличии: " + Quantity + " шт.";
            }
        }
    }
}
