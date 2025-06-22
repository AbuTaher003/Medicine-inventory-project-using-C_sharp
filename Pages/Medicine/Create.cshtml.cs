using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MedicineInventory.Pages.Medicine
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public MedicineData Medicine { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int? userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null)
            {
                // যদি session না থাকে, তাহলে login page-এ redirect
                return RedirectToPage("/Login");
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using var conn = new MySqlConnection(connStr);
            conn.Open();

            // Medicine Insert Query
            string sql = "INSERT INTO medicines (name, manufacturer, quantity, expiry_date) VALUES (@name, @manufacturer, @quantity, @expiry_date)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", Medicine.Name);
            cmd.Parameters.AddWithValue("@manufacturer", Medicine.Manufacturer ?? "");
            cmd.Parameters.AddWithValue("@quantity", Medicine.Quantity);
            cmd.Parameters.AddWithValue("@expiry_date", Medicine.ExpiryDate.HasValue ? Medicine.ExpiryDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);

            cmd.ExecuteNonQuery();

            // নতুন medicine এর ID
            long newMedicineId = cmd.LastInsertedId;

            // Audit log insert
            string auditSql = "INSERT INTO audit_logs (user_id, medicine_id, action, action_time) VALUES (@userId, @medicineId, 'added', NOW())";
            using var auditCmd = new MySqlCommand(auditSql, conn);
            auditCmd.Parameters.AddWithValue("@userId", userId.Value); // Session থেকে পাওয়া ID
            auditCmd.Parameters.AddWithValue("@medicineId", newMedicineId);

            auditCmd.ExecuteNonQuery();

            return RedirectToPage("Index");
        }

        public class MedicineData
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }

            public string Manufacturer { get; set; }

            [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or more")]
            public int Quantity { get; set; }

            [DataType(DataType.Date)]
            public DateTime? ExpiryDate { get; set; }
        }
    }
}
