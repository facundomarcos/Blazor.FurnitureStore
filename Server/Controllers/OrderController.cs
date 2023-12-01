using Blazor.FurnitureStore.Repositories;
using Blazor.FurnitureStore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace Blazor.FurnitureStore.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductRepository _orderProductRepository;

        public OrderController(IOrderRepository orderRepository, 
            IOrderProductRepository orderProductRepository)
        {
            _orderRepository = orderRepository;
            _orderProductRepository = orderProductRepository;

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            if (order.OrderNumber == 0)
                ModelState.AddModelError("OrderNumber", "Order number can't be empty");

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            using(TransactionScope scope =  new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) 
            {
                order.Id = await _orderRepository.GetNextId();

                await _orderRepository.InsertOrder(order);

                if(order.Products != null && order.Products.Any())
                {
                    foreach (var prd in order.Products)
                    {
                        //insert products
                        await _orderProductRepository.InsertOrderProduct(order.Id, prd);
                    }
                }
                scope.Complete();
               
            }



            return NoContent();
        }

        [HttpGet("GetNextNumber")]
        public async Task<int> GetNextNumber()
        {
            return await _orderRepository.GetNextNumber();
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
         return  await  _orderRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<Order> GetDetails(int id)
        {
            var order = await _orderRepository.GetDetails(id);

            var products = await _orderProductRepository.GetByOrder(id);

            if(order != null)
            {
                order.Products = products.ToList();
            }
            return order;
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _orderRepository.DeleteOrder(id);
        }
    }
}
