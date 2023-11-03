using System;
using System.Collections.Generic;
using System.Text;

namespace TestApi.Pages
{
    public class OrderModel
    {
        public string Phone { get; set; }
        public string FIO { get; set; }
        public string ZakazFromAddress { get; set; }
        public string ZakazToAddress { get; set; }
        public string ZakazToAddressDop { get; set; }
        public string ZakazComment { get; set; }
        public string Sdacha { get; set; }
        public string Dostavka { get; set; }
        public string Summ { get; set; }
        public int CheckDostavka { get; set; }
        public int TypeOplata { get; set; }
        public int TypeZakaz { get; set; }
        public List<int> Estab { get; set; }
        public List<int> Product { get; set; }
    }
}
