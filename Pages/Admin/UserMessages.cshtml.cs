using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace MedicineInventory.Pages.Admin
{
    public class UserMessagesModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<UserMessage> Messages { get; set; } = new();

        public UserMessagesModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = "SELECT id, name, email, message, submitted_at FROM contact_messages ORDER BY submitted_at DESC";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Messages.Add(new UserMessage
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Email = reader.GetString("email"),
                    Message = reader.GetString("message"),
                    SubmittedAt = reader.GetDateTime("submitted_at"),
                });
            }
        }

        public class UserMessage
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public DateTime SubmittedAt { get; set; }
        }
    }
}
