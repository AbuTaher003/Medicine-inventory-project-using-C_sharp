using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace MedicineInventory.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ContactModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public ContactForm Contact { get; set; }

        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using var conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = "INSERT INTO contact_messages (name, email, message, submitted_at) VALUES (@name, @email, @message, NOW())";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", Contact.Name);
            cmd.Parameters.AddWithValue("@email", Contact.Email);
            cmd.Parameters.AddWithValue("@message", Contact.Message);
            cmd.ExecuteNonQuery();

            SuccessMessage = "Your message has been sent successfully!";
            ModelState.Clear();
            Contact = new ContactForm(); // Clear the form

            return Page();
        }

        public class ContactForm
        {
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Message is required")]
            public string Message { get; set; }
        }
    }
}
