using CarpetStore.Models.Interfaces;
using CarpetStore.Models;
using CarpetStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarpetStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public ReportsController(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Daily(DateTime? date = null)
        {
            var targetDate = date ?? DateTime.Today;
            var startDate = targetDate.Date;
            var endDate = startDate.AddDays(1).AddTicks(-1);

            var orders = _orderRepository.GetAllOrders()
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var report = GenerateReport(orders, startDate, endDate, "Daily");
            report.StartDate = startDate;
            report.EndDate = endDate;

            return View("Report", report);
        }

        public IActionResult Weekly(DateTime? startDate = null)
        {
            var targetDate = startDate ?? DateTime.Today;
            var weekStart = targetDate.Date.AddDays(-(int)targetDate.DayOfWeek);
            var weekEnd = weekStart.AddDays(7).AddTicks(-1);

            var orders = _orderRepository.GetAllOrders()
                .Where(o => o.OrderDate >= weekStart && o.OrderDate <= weekEnd)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var report = GenerateReport(orders, weekStart, weekEnd, "Weekly");
            report.StartDate = weekStart;
            report.EndDate = weekEnd;

            return View("Report", report);
        }

        public IActionResult Monthly(DateTime? date = null)
        {
            var targetDate = date ?? DateTime.Today;
            var monthStart = new DateTime(targetDate.Year, targetDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

            var orders = _orderRepository.GetAllOrders()
                .Where(o => o.OrderDate >= monthStart && o.OrderDate <= monthEnd)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var report = GenerateReport(orders, monthStart, monthEnd, "Monthly");
            report.StartDate = monthStart;
            report.EndDate = monthEnd;

            return View("Report", report);
        }

        public IActionResult Custom(DateTime? startDate = null, DateTime? endDate = null, int? year = null, int? month = null)
        {
            DateTime start, end;

            // If year and month are provided, use them to set date range
            if (year.HasValue && month.HasValue && month > 0)
            {
                start = new DateTime(year.Value, month.Value, 1);
                end = start.AddMonths(1).AddDays(-1);
            }
            else if (year.HasValue)
            {
                start = new DateTime(year.Value, 1, 1);
                end = new DateTime(year.Value, 12, 31);
            }
            else
            {
                start = startDate ?? DateTime.Today.AddDays(-30);
                end = endDate ?? DateTime.Today;
            }

            var orders = _orderRepository.GetAllOrders()
                .Where(o => o.OrderDate >= start && o.OrderDate <= end)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var report = GenerateReport(orders, start, end, "Custom");
            report.StartDate = start;
            report.EndDate = end;

            return View("Report", report);
        }

        private OrderReportViewModel GenerateReport(List<Order> orders, DateTime startDate, DateTime endDate, string reportType)
        {
            var report = new OrderReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                ReportType = reportType,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.OrderTotal),
                Orders = orders
            };

            report.AverageOrderValue = report.TotalOrders > 0 ? report.TotalRevenue / report.TotalOrders : 0;

            // Generate order summaries based on report type
            report.OrderSummaries = GenerateOrderSummaries(orders, startDate, endDate, reportType);

            // Generate top selling products
            report.TopSellingProducts = GenerateTopSellingProducts(orders);

            // Generate status summaries
            report.StatusSummaries = GenerateStatusSummaries(orders);

            return report;
        }

        private List<OrderSummary> GenerateOrderSummaries(List<Order> orders, DateTime startDate, DateTime endDate, string reportType)
        {
            var summaries = new List<OrderSummary>();

            switch (reportType)
            {
                case "Daily":
                    // Group by hour for daily report
                    for (int hour = 0; hour < 24; hour++)
                    {
                        var hourStart = startDate.AddHours(hour);
                        var hourEnd = hourStart.AddHours(1).AddTicks(-1);
                        var hourOrders = orders.Where(o => o.OrderDate >= hourStart && o.OrderDate <= hourEnd).ToList();
                        
                        summaries.Add(new OrderSummary
                        {
                            Date = hourStart,
                            OrderCount = hourOrders.Count,
                            Revenue = hourOrders.Sum(o => o.OrderTotal),
                            PeriodLabel = hourStart.ToString("HH:00")
                        });
                    }
                    break;

                case "Weekly":
                    // Group by day for weekly report
                    for (int day = 0; day < 7; day++)
                    {
                        var dayStart = startDate.AddDays(day);
                        var dayEnd = dayStart.AddDays(1).AddTicks(-1);
                        var dayOrders = orders.Where(o => o.OrderDate >= dayStart && o.OrderDate <= dayEnd).ToList();
                        
                        summaries.Add(new OrderSummary
                        {
                            Date = dayStart,
                            OrderCount = dayOrders.Count,
                            Revenue = dayOrders.Sum(o => o.OrderTotal),
                            PeriodLabel = dayStart.ToString("ddd, MMM dd")
                        });
                    }
                    break;

                case "Monthly":
                    // Group by week for monthly report
                    var currentDate = startDate;
                    int weekNumber = 1;
                    while (currentDate < endDate)
                    {
                        var weekEnd = currentDate.AddDays(7);
                        if (weekEnd > endDate) weekEnd = endDate;
                        
                        var weekOrders = orders.Where(o => o.OrderDate >= currentDate && o.OrderDate <= weekEnd).ToList();
                        
                        summaries.Add(new OrderSummary
                        {
                            Date = currentDate,
                            OrderCount = weekOrders.Count,
                            Revenue = weekOrders.Sum(o => o.OrderTotal),
                            PeriodLabel = $"Week {weekNumber} ({currentDate:MMM dd} - {weekEnd.AddDays(-1):MMM dd})"
                        });
                        
                        currentDate = weekEnd;
                        weekNumber++;
                    }
                    break;

                case "Custom":
                    // Group by day for custom report
                    var days = (endDate - startDate).Days + 1;
                    for (int day = 0; day < days; day++)
                    {
                        var dayStart = startDate.AddDays(day);
                        var dayEnd = dayStart.AddDays(1).AddTicks(-1);
                        var dayOrders = orders.Where(o => o.OrderDate >= dayStart && o.OrderDate <= dayEnd).ToList();
                        
                        summaries.Add(new OrderSummary
                        {
                            Date = dayStart,
                            OrderCount = dayOrders.Count,
                            Revenue = dayOrders.Sum(o => o.OrderTotal),
                            PeriodLabel = dayStart.ToString("MMM dd, yyyy")
                        });
                    }
                    break;
            }

            return summaries;
        }

        private List<ProductSales> GenerateTopSellingProducts(List<Order> orders)
        {
            var productSales = new Dictionary<int, ProductSales>();

            foreach (var order in orders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    if (productSales.ContainsKey(detail.ProductId))
                    {
                        productSales[detail.ProductId].QuantitySold += detail.Quantity;
                        productSales[detail.ProductId].Revenue += detail.Price * detail.Quantity;
                    }
                    else
                    {
                        var product = _productRepository.GetProductById(detail.ProductId);
                        productSales[detail.ProductId] = new ProductSales
                        {
                            ProductId = detail.ProductId,
                            ProductName = product?.Name ?? "Unknown Product",
                            Category = product?.Category ?? "Unknown",
                            QuantitySold = detail.Quantity,
                            Revenue = detail.Price * detail.Quantity
                        };
                    }
                }
            }

            return productSales.Values
                .OrderByDescending(p => p.Revenue)
                .Take(10)
                .ToList();
        }

        private List<StatusSummary> GenerateStatusSummaries(List<Order> orders)
        {
            var statusGroups = orders.GroupBy(o => o.OrderStatus)
                .Select(g => new StatusSummary
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Revenue = g.Sum(o => o.OrderTotal),
                    Percentage = orders.Count > 0 ? (double)g.Count() / orders.Count * 100 : 0
                })
                .OrderByDescending(s => s.Count)
                .ToList();

            return statusGroups;
        }
    }
}
