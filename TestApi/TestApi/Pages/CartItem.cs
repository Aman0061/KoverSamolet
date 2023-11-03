using System;
using System.Collections.Generic;
using System.Text;

namespace TestApi.Pages
{
    public class CartItem
    {
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }

        public int CodeEstablishment { get; set; }

        public string EstablishmentCode { get; set; } // Добавляем свойство EstablishmentCode

        public string EstablishmentName { get; set; } // Название заведения
        public string FoodCodeid { get; set; } // Название заведения
    }
}
