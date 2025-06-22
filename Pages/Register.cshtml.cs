using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace MedicineInventory.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public RegisterModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string Role { get; set; } = "user";

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
            // ডিফল্ট role "user"
            Role = "user";
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Role))
            {
                Message = "All fields are required.";
                return Page();
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "INSERT INTO users (username, password, role) VALUES (@username, @password, @role)";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", Username);
                        // BCrypt.Net-Next এর HashPassword ব্যবহার
                        cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(Password));
                        cmd.Parameters.AddWithValue("@role", Role);

                        cmd.ExecuteNonQuery();
                    }
                }
                Message = "Registration successful! You can now log in.";
                ModelState.Clear();
            }
            catch (MySqlException ex)
            {
                Message = "Database error: " + ex.Message;
            }
            catch (Exception ex)
            {
                Message = "Unexpected error: " + ex.Message;
            }

            return Page();
        }
    }
}
