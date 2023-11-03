using System;
using System.Collections.Generic;
using System.Text;

namespace TestApi.Pages
{
    public class RestaurantCartItemGroup
{
    public string RestaurantName { get; set; }
    public List<CartItem> Items { get; set; }
}
}
