using CarpetStore.Models;

namespace CarpetStore.ViewModels
{
    public class OrderReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReportType { get; set; } = string.Empty; // Daily, Weekly, Monthly
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<OrderSummary> OrderSummaries { get; set; } = new List<OrderSummary>();
        public List<ProductSales> TopSellingProducts { get; set; } = new List<ProductSales>();
        public List<StatusSummary> StatusSummaries { get; set; } = new List<StatusSummary>();
    }

    public class OrderSummary
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public string PeriodLabel { get; set; } = string.Empty;
    }

    public class ProductSales
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class StatusSummary
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
        public double Percentage { get; set; }
    }
}









