using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace MedicineInventory.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Role { get; set; }

        public string Message { get; set; }

        public void OnGet() { }

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
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();

                    string sql = "SELECT * FROM users WHERE username=@username AND role=@role";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", Username);
                        cmd.Parameters.AddWithValue("@role", Role);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string dbPassword = reader.GetString("password");
                                if (BCrypt.Net.BCrypt.Verify(Password, dbPassword))
                                {
                                    int userId = reader.GetInt32("id");

                                    // Clear old session and set new one
                                    HttpContext.Session.Clear();
                                    HttpContext.Session.SetInt32("user_id", userId);
                                    HttpContext.Session.SetString("username", Username);
                                    HttpContext.Session.SetString("role", Role);

                                    return Role switch
                                    {
                                        "Admin" => RedirectToPage("/Admin/Dashboard"),
                                        "User" => RedirectToPage("/User/Dashboard"),
                                        "Auditor" => RedirectToPage("/Auditor/Dashboard"),
                                        _ => RedirectToPage("/Error")
                                    };
                                }
                                else
                                {
                                    Message = "Wrong Password.";
                                }
                            }
                            else
                            {
                                Message = "User not found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "Server error: " + ex.Message;
            }

            return Page();
        }
    }
}
