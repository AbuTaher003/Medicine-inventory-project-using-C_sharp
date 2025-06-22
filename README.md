- ``` You can download the Presentation Slides for this project from here``` [Download](https://docs.google.com/presentation/d/1o1pFBj18eSO7-C2UzAJpy-ukpZpdywPu/edit?usp=drive_link&ouid=109714297562801142424&rtpof=true&sd=true)
- ``` You can also download the .Zip file from here``` [Download](https://drive.google.com/file/d/1Re2jKFzXBWp4PHeXwulic8ZZ4wtUcWDd/view?usp=drive_link)


# Medicine Inventory Management System

A simple and efficient web-based system built with **ASP.NET Core Razor Pages** and **MySQL**, designed to manage medicine stock, track inventory, generate reports, and maintain audit trails.

---

## 📌 Features

- Role-based login (Admin, User, Auditor)
- Add, edit, delete, and view medicines
- Expiry date and low stock tracking
- Audit logs for accountability
- Contact form with message storage
- Report generation (e.g., low stock report)

---

## 🛠️ Technologies Used

- ASP.NET Core Razor Pages (.NET 7)
- C#
- Entity Framework Core
- MySQL (XAMPP / phpMyAdmin)
- HTML, CSS
- Visual Studio / Visual Studio Code

---

## 🗃️ Database Setup

Use the following SQL script to set up your MySQL database:

```sql
CREATE DATABASE IF NOT EXISTS medicine_inventory 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE medicine_inventory;

CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    role ENUM('admin', 'user', 'auditor') NOT NULL DEFAULT 'user',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS medicines (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    manufacturer VARCHAR(255),
    quantity INT DEFAULT 0,
    expiry_date DATE NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS audit_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    medicine_id INT NOT NULL,
    action ENUM('added', 'edited', 'deleted') NOT NULL,
    action_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (medicine_id) REFERENCES medicines(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS reports (
    id INT AUTO_INCREMENT PRIMARY KEY,
    report_name VARCHAR(255) NOT NULL,
    generated_by INT,
    generated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    report_data TEXT,
    FOREIGN KEY (generated_by) REFERENCES users(id) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS contact_messages (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL,
    message TEXT NOT NULL,
    submitted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```
---


❓How to Run the Project
- ```Clone this repository.```

- ```Set up the MySQL database using the script above.```

- ```Configure your connection string in appsettings.json:```

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=medicine_inventory;Uid=root;Pwd=your_password;"
}
```
---
📂 Folder Structure
```pgsql
/Pages
  ├── Index.cshtml
  ├── Medicines/
  ├── AuditLogs/
  ├── Reports/
  ├── Contact/
  └── Shared/
  
/Models
  ├── User.cs
  ├── Medicine.cs
  ├── AuditLog.cs
  ├── Report.cs
  └── ContactMessage.cs
```
