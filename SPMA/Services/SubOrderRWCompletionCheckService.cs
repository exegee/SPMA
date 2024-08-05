using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SPMA.Controllers.Orders;
using SPMA.Data;
using SPMA.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SPMA.Services
{
    public class SubOrderRWCompletionCheckService : IHostedService, IDisposable
    {
        private Timer _timer;
        public string stan;
        // Interval in seconds
        private int _interval = 1800;
        public bool IsRunning { get; private set; } = false;
        public string Job { get; private set; } = "Idle";
        private readonly IServiceScopeFactory _scopeFactory;
        public SubOrderRWCompletionCheckService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public SubOrderRWCompletionCheckService()
        {
            StartAsync(new CancellationToken());
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
                GetServiceSettings();
                _timer = new Timer(t => {
                //Create a scope to use service within service
                using(var scope = _scopeFactory.CreateScope())
                {
                        stan = "Działa!";
                    UpdateServiceStatus(true, "Updating...");
                    // Get ApplicationDbContext service
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var orderController = scope.ServiceProvider.GetRequiredService<OrdersController>();

                    List<Order> orders = context.Orders
                                            .Select(o => new Order
                                            {
                                               OrderId = o.OrderId,
                                               State = o.State,
                                               RwCompletion = o.RwCompletion
                                           }).Where(item => item.State != 10).ToList();
                    context.AttachRange(orders);
                    foreach (Order order in orders)
                    {
                        order.RwCompletion = (decimal)(orderController.GetRWStatus(order.OrderId) as OkObjectResult).Value;
                    }
                    context.SaveChanges();
                    UpdateServiceStatus(true, "Idle");
                        stan = "Koniec";
                    }
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(_interval));
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            UpdateServiceStatus(false, "Stopped");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates service status in database
        /// </summary>
        /// <param name="status"></param>
        private void UpdateServiceStatus(bool status, string message)
        {
            // Update service status
            using (var scope = _scopeFactory.CreateScope())
            {
                string className = GetType().Name;
                string serviceName = className.Substring(0, className.Length - 7);
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var service = context.Services.Where(s => s.Name == serviceName).SingleOrDefault();
                service.CurrentJob = message;
                service.IsRunning = status;
                context.SaveChanges();
            }
            IsRunning = status;
            Job = message;
        }

        /// <summary>
        /// Get new settings for service
        /// </summary>
        private void GetServiceSettings()
        {
            // Get latest settings for this service
            using (var scope = _scopeFactory.CreateScope())
            {
                string className = GetType().Name;
                string serviceName = className.Substring(0, className.Length - 7);
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _interval = context.Services.Where(s => s.Name == serviceName).Select(s => s.RunInterval).SingleOrDefault();
            }
        } 
    }
}
