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
        private readonly IGenericRepository<CurrentStock> _currentRepository;
        private readonly IGenericRepository<Sale> _saleRepository;
        private readonly IGenericRepository<SaleReport> _saleReportRepository;

        public OrderController(IGenericRepository<Order> orderRepository,
                               IGenericRepository<OrderItem> orderItemRepository,
                               IGenericRepository<Order_CurrentStock> orderCurrentRepository,
                               IGenericRepository<CurrentStock> currentRepository,
                               IGenericRepository<Sale> saleRepository,
                                IGenericRepository<SaleReport> saleReportRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderCurrentRepository = orderCurrentRepository;
            _currentRepository = currentRepository;
            _saleRepository = saleRepository;
            _saleReportRepository = saleReportRepository;
        }

        public IQueryable<Order> Get()
        {
            //return _orderRepository.GetAll(or => or.Customer, or => or.FieldAgent).Where(p=> p.Pending == true);
            return _orderRepository.GetAll(or => or.Customer, or => or.FieldAgent);
        }

        public IQueryable<OrderItem> GetItem()
        {
            return _orderItemRepository.GetAll();
        }

        public IQueryable<OrderItem> GetSpecificItem(Guid id)
        {
            return _orderItemRepository.GetAll(c=> c.CompanyStockTag, c=> c.Order).Where(oi => oi.OrderId == id);
        }

        public bool GetOrderApproval(Guid id, Guid id2)
        {
            var orderForUpdate = _orderRepository.GetAll(c=> c.OrderItems).Where(oi => oi.id == id).FirstOrDefault();
            if (orderForUpdate == null) throw new ArgumentNullException("order");

            // update Order data
            orderForUpdate.Approved = true;
            orderForUpdate.Pending = false;
            _orderRepository.Update(orderForUpdate);

            // save sales and sales report
            Sale saleEntity = new Sale();
            saleEntity.id = Guid.NewGuid();
            saleEntity.OrderId = orderForUpdate.id;
            saleEntity.AmountCollected = orderForUpdate.TotalOrderAmount;
            saleEntity.CollectorId = id2;
            saleEntity.DateCreated = DateTime.Now;

            var savedEntity = _saleRepository.Add(saleEntity);

            // calculate Sales Profit
            var salesProfit = orderForUpdate.TotalOrderAmount - orderForUpdate.TotalSuppliedAmount;
            // save sales and sales report
            SaleReport saleReportEntity = new SaleReport();
            saleReportEntity.id = Guid.NewGuid();
            saleReportEntity.SaleId = saleEntity.id;
            saleReportEntity.AmountCollected = saleEntity.AmountCollected;
            saleReportEntity.SuppliedAmount = orderForUpdate.TotalSuppliedAmount;
            saleReportEntity.ProfitMade = salesProfit;
            saleReportEntity.DateCreated = DateTime.Now;

            _saleReportRepository.Add(saleReportEntity);

            // update current stock accordingly
            foreach (var ord2 in orderForUpdate.OrderItems)
            {
                // Get ordered product from stock
                var currentProduct = _currentRepository.GetAll()
                    .Where(pd => pd.StockNameId == ord2.ProductId).FirstOrDefault();

                //deduct ordered quantity and update
                int soldStock = ord2.Quantity;
                int quantityInStock = currentProduct.Quantity;
                int balance = quantityInStock - soldStock;

                // update current stock in quantity
                currentProduct.Quantity = balance;
                // update order stock
                _currentRepository.Update(currentProduct);

            }

            return true;
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]OrderItem[] orderItems)
        {
            //if (!ModelState.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest);            

            decimal count = 0;
            decimal count2 = 0;
            // calculate total sales amount and supplied amount
            foreach (var ord in orderItems)
            {                
                count = count + ord.SalesTotalAmount;
                count2 = count2 + ord.SuppliedTotalPrice; // included this recently
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
            orderEntity.TotalSuppliedAmount = count2; // included this recently
            //orderEntity.AgentId = Guid.Parse("E836F4E6-86FA-4518-B1DD-5D89A62226F4"); // item orderid holds the agent id - local
            orderEntity.AgentId = Guid.Parse("EF629817-3DB3-4EF2-901F-202C6C07EFA6");
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
            if (order == null) throw new ArgumentNullException("order");

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
