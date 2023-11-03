using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApi.Pages
{
    public class CartManager
    {
        private static CartManager instance;
        private List<CartItem> cartItems = new List<CartItem>();

        private CartManager()
        {
            // Приватный конструктор, чтобы предотвратить создание экземпляров извне.
        }

        public static CartManager GetInstance()
        {
            if (instance == null)
            {
                instance = new CartManager();
            }
            return instance;
        }

        // Добавьте метод для добавления товара в корзину
        public void AddToCart(CartItem cartItem)
        {
            cartItems.Add(cartItem);
        }

        // Метод для удаления товара из корзины
        public void RemoveFromCart(CartItem cartItem)
        {
            cartItems.Remove(cartItem);
        }

        // Метод для получения списка товаров в корзине
        public List<CartItem> GetCartItems()
        {
            return cartItems;
        }

        public decimal CalculateTotalAmount()
        {
            // Используйте LINQ для вычисления общей суммы заказа
            decimal totalAmount = cartItems.Sum(item => item.ProductPrice * item.Quantity);
            return totalAmount;
        }


        public List<int> GetUniqueEstablishments()
        {
            List<int> uniqueEstablishments = new List<int>();

            foreach (var item in CartManager.GetInstance().GetCartItems())
            {
                if (!uniqueEstablishments.Contains(item.CodeEstablishment))
                {
                    uniqueEstablishments.Add(item.CodeEstablishment);
                }
            }

            return uniqueEstablishments;
        }


        // Остальной код класса CartManager...
    }
}
