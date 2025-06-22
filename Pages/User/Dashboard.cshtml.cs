using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MedicineInventory.Pages.User
{
    public class DashboardModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DashboardModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Username { get; set; } = "User";
        public string SearchTerm { get; set; }

        public List<MedicineInfo> SearchResults { get; set; } = new();
        public List<MedicineInfo> AllMedicines { get; set; } = new();
        public List<MedicineInfo> ExpiringSoonMedicines { get; set; } = new();

        public void OnGet(string searchTerm)
        {
            SearchTerm = searchTerm;

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            // ✅ Username from Session
            var sessionUsername = HttpContext.Session.GetString("username");
            if (!string.IsNullOrEmpty(sessionUsername))
                Username = sessionUsername;

            // 🔍 Search functionality
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string searchQuery = "SELECT name, manufacturer, quantity FROM medicines WHERE name LIKE @term OR manufacturer LIKE @term";
                using var searchCmd = new MySqlCommand(searchQuery, conn);
                searchCmd.Parameters.AddWithValue("@term", $"%{SearchTerm}%");

                using var reader = searchCmd.ExecuteReader();
                while (reader.Read())
                {
                    SearchResults.Add(new MedicineInfo
                    {
                        Name = reader.GetString("name"),
                        Manufacturer = reader.GetString("manufacturer"),
                        Quantity = reader.GetInt32("quantity")
                    });
                }
                reader.Close();
            }

            // 📋 All Medicines
            string allQuery = "SELECT name, manufacturer, quantity FROM medicines ORDER BY name";
            using var allCmd = new MySqlCommand(allQuery, conn);
            using var allReader = allCmd.ExecuteReader();
            while (allReader.Read())
            {
                AllMedicines.Add(new MedicineInfo
                {
                    Name = allReader.GetString("name"),
                    Manufacturer = allReader.GetString("manufacturer"),
                    Quantity = allReader.GetInt32("quantity")
                });
            }
            allReader.Close();

            // ⏰ Expiring Soon
            string expiryQuery = "SELECT name, expiry_date FROM medicines WHERE expiry_date <= DATE_ADD(CURDATE(), INTERVAL 30 DAY)";
            using var expiryCmd = new MySqlCommand(expiryQuery, conn);
            using var expiryReader = expiryCmd.ExecuteReader();
            while (expiryReader.Read())
            {
                ExpiringSoonMedicines.Add(new MedicineInfo
                {
                    Name = expiryReader.GetString("name"),
                    ExpiryDate = expiryReader.GetDateTime("expiry_date")
                });
            }
        }

        public class MedicineInfo
        {
            public string Name { get; set; }
            public string Manufacturer { get; set; }
            public int Quantity { get; set; }
            public DateTime ExpiryDate { get; set; }
        }
    }
}
