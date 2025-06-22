using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace MedicineInventory.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public int LowStockCount { get; set; }
        public List<MedicineAlert> LowStockMedicines { get; set; } = new();
        
        public List<MedicineAlert> ExpiryAlerts { get; set; } = new();

        public DashboardModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();

                // Low stock medicines count and details (quantity < 10)
                string lowStockSql = "SELECT id, name, quantity FROM medicines WHERE quantity < 10 ORDER BY quantity ASC LIMIT 5";
                using (var lowStockCmd = new MySqlCommand(lowStockSql, conn))
                using (var reader = lowStockCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LowStockMedicines.Add(new MedicineAlert
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Quantity = reader.GetInt32("quantity"),
                        });
                    }
                }
                LowStockCount = LowStockMedicines.Count;

                // Expiry alert medicines (expired or near expiry within 30 days)
                string expirySql = @"SELECT id, name, expiry_date 
                                     FROM medicines 
                                     WHERE expiry_date IS NOT NULL 
                                     AND expiry_date <= DATE_ADD(CURDATE(), INTERVAL 30 DAY)
                                     ORDER BY expiry_date ASC";
                using (var expiryCmd = new MySqlCommand(expirySql, conn))
                using (var reader2 = expiryCmd.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        ExpiryAlerts.Add(new MedicineAlert
                        {
                            Id = reader2.GetInt32("id"),
                            Name = reader2.GetString("name"),
                            ExpiryDate = reader2.GetDateTime("expiry_date"),
                        });
                    }
                }
            }
        }

        public class MedicineAlert
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public DateTime? ExpiryDate { get; set; }
        }
    }
}
