# 🏥 HR Management System with Advanced Reporting

A modern **HR Management System** built with enterprise-level architecture, featuring multi-company management, employee tracking, and dynamic PDF report generation.

---

## 🚀 Key Features

* **Company Management:** Multi-company support with unique salary percentage configurations (Basic, HR, Medical).
* **Employee & Attendance:** Comprehensive employee data management and shift-based attendance tracking.
* **Advanced Reporting:** * Employee List (Filtered by Department)
    * Attendance Summary with Real-time Preview
    * Detailed Salary breakdown (Basic, HR, MA, Gross, Payable)
    * Monthly Salary Summary Report
* **Technology Stack:**
    * **Backend:** ASP.NET Core 8.0
    * **Database:** PostgreSQL (utilizing pgcrypto for data security)
    * **Architecture:** Repository Pattern & Unit of Work
    * **UI:** jQuery, AJAX, Bootstrap 5, and FontAwesome
    * **Reporting:** QuestPDF (High-performance PDF generation engine)

---

## 🛠️ Technical Stack

| Category | Technology |
| :--- | :--- |
| **Framework** | ASP.NET Core 8 (MVC) |
| **Database** | PostgreSQL |
| **Pattern** | Unit of Work & Repository Pattern |
| **Frontend** | jQuery, AJAX, Razor Pages |
| **PDF Engine** | QuestPDF |
| **Tools** | Visual Studio 2022, GitHub |

---

## 🗄️ Database Setup

For your convenience, a complete database script has been included in the project.
* **Location:** You can find the SQL script in the `/dbScript` folder.
* **Content:** This folder contains the table structures, extensions, and initial data required to run the project.

---

## ⚙️ Setup Guide

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/A-N-Shuvo/HR-Management-System.git](https://github.com/A-N-Shuvo/HR-Management-System.git)
    ```
2.  **Restore Database:** Execute the script found in the `/dbScript` folder on your PostgreSQL server.
3.  **Configure Connection:** Update the connection string in the `appsettings.json` file.
4.  **Apply Migrations:** Run the following command in the Package Manager Console:
    ```powershell
    Update-Database
    ```
5.  **Run:** Press `F5` in Visual Studio.

---

## 👨‍💻 Author
**A. N. Shuvo** 
*.NET Developer*

---

> **Note:** This project is developed as a special assignment focusing on clean code, reusable logic, and high-performance reporting.
