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

        public OrderController(IGenericRepository<Order> orderRepository,
                               IGenericRepository<OrderItem> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public IQueryable<Order> Get()
        {
            return _orderRepository.GetAll();
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]Order order)
        {
            if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);
            order.id = Guid.NewGuid();
            order.DateCreated = DateTime.Now;
            
            // Save Order data
            var savedEntity = _orderRepository.Add(order);

            // Save each order items
            foreach (var ord in order.OrderItems)
            {
                ord.id = Guid.NewGuid();
                ord.OrderId = order.id;
                var itemEntity = _orderItemRepository.Add(ord);
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
