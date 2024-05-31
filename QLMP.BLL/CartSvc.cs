using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLMP.DAL;
using QLMP.Common.Rsp;
using QLMP.Common.BLL;
using QLMP.DAL.Models;

namespace QLMP.BLL
{
    public class CartSvc : GenericSvc<CartRep, Cart>
    {
        private CartRep cartRep;
        public CartSvc()
        {
            cartRep = new CartRep();
        }

        public SingleRsp AddProductToCart(int Makh, int productId, int quantity)
        {
            var res = new SingleRsp();
            var cart = cartRep.GetCartByUserId(Makh);
            if (Makh == 0)
            {
                res.SetError("Can not Add");
                return res;
            }
            if (cart == null)
            {
                cart = new Cart { UserId = Makh };
                cartRep.Create(cart);
                cart = cartRep.GetCartByUserId(Makh); // Ensure the cart is reloaded with an ID
            }

            var cartItem = cartRep.AddProductToCart(cart.Id, productId, quantity);
            res.Data = cartItem;
            return res;
           
        }

        public SingleRsp PlaceOrder(int userId)
        {
            return cartRep.PlaceOrder(userId);
        }
        public SingleRsp GetCartById(int Makh)
        {
            return cartRep.GetCartById(Makh);
        }

        public SingleRsp RemoveProductFromCart(int cartItemId)
        {
            return cartRep.RemoveProductFromCart(cartItemId);
        }
        public SingleRsp GetOrderById(int userId)
        {
            return cartRep.GetOrderById(userId);
        }
        public SingleRsp GetOrderByOrderId(int orderId)
        {
            return cartRep.GetOrderByOrderId(orderId);
        }
        public SingleRsp GetAllOrders()
        {
            return cartRep.GetAllOrders();
        }
        public SingleRsp GetSalesStatisticsByProductType()
        {
            return cartRep.GetSalesStatisticsByProductType();
        }
        public SingleRsp GetMonthlySalesStatistics()
        {
            return cartRep.GetMonthlySalesStatistics();
        }
        public SingleRsp GetRecentOrders()
        {
            return cartRep.GetRecentOrders();
        }
    }

}

