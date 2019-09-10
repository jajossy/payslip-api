using BaseWebApi.Models;
using BaseWebApi.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BaseWebApi.Controllers
{
    public class OrderController : ApiController
    {
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly IGenericRepository<Order_CurrentStock> _orderCurrentRepository;

        public OrderController(IGenericRepository<Order> orderRepository,
                               IGenericRepository<OrderItem> orderItemRepository,
                               IGenericRepository<Order_CurrentStock> orderCurrentRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderCurrentRepository = orderCurrentRepository;
        }

        public IQueryable<Order> Get()
        {
            return _orderRepository.GetAll();
        }

        public IQueryable<OrderItem> GetItem()
        {
            return _orderItemRepository.GetAll();
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]OrderItem[] orderItems)
        {
            //if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);            

            decimal count = 0;
            // calculate total sales amount
            foreach (var ord in orderItems)
            {                
                count = count + ord.SalesTotalAmount;
            }

            // Compute and Save Order data

            Order orderEntity = new Order();
            orderEntity.id = Guid.NewGuid();

            // use the date and time to created a unique hash tag for order
            DateTime _now = DateTime.Now;
            string _dd = _now.ToString("dd"); //
            string _mm = _now.ToString("MM");
            string _yy = _now.ToString("yyyy");
            string _hh = _now.Hour.ToString();
            string _min = _now.Minute.ToString();
            string _ss = _now.Second.ToString();

            string _uniqueId = _dd + _hh + _mm + _min + _ss + _yy;
            orderEntity.OrderTag = "#" + _uniqueId;

            orderEntity.DateCreated = DateTime.Now;
            orderEntity.TotalOrderAmount = count;
            orderEntity.AgentId = Guid.Parse("EF629817-3DB3-4EF2-901F-202C6C07EFA6"); // item orderid holds the agent id
            orderEntity.CustomerId = orderItems[0].id;
            orderEntity.PaymentType = orderItems[0].BatchNo;
            orderEntity.Pending = true;
            var savedEntity = _orderRepository.Add(orderEntity);

            // Save each order items with order id
            foreach (var ord in orderItems)
            {
                ord.id = Guid.NewGuid();
                ord.OrderId = orderEntity.id;
                ord.BatchNo = "";
                var itemEntity = _orderItemRepository.Add(ord);                
            }

            // update order current stock accordingly
            foreach (var ord2 in orderItems)
            {
                // Get ordered product from stock
                var orderCurrentProduct = _orderCurrentRepository.GetAll()
                    .Where(pd => pd.StockNameId == ord2.ProductId).FirstOrDefault();
                
                //deduct ordered quantity and update
                int soldStock = ord2.Quantity;
                int quantityInStock  = orderCurrentProduct.Quantity;
                int balance = quantityInStock - soldStock;

                // update current stock in quantity
                orderCurrentProduct.Quantity = balance;
                // update order stock
                _orderCurrentRepository.Update(orderCurrentProduct);

            }
            
            var response = Request.CreateResponse(HttpStatusCode.Created, savedEntity);         

            return response;
        }

        [HttpPut]
        public void Put([FromBody]Order order)
        {
            if (order == null) throw new ArgumentNullException("agent");

            // update Order data
            _orderRepository.Update(order);
        }

        [HttpDelete]
        public void Delete(Guid id)
        {
            var order = _orderRepository.GetById(id);

            if (order == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _orderRepository.Remove(order);
        }
    }
}
