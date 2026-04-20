HR Management System (ASP.NET Core 8 & PostgreSQL)
A comprehensive Human Resources Management System built to streamline employee tracking, attendance, and payroll processing for multiple companies.

🚀 Technologies Used
Framework: ASP.NET Core 8

Database: PostgreSQL

Architecture: Repository Pattern & Unit of Work

Frontend: jQuery & Ajax (for seamless CRUD operations)

Reporting: QuestPDF / RDLC (Tabular format reports)

Authentication: JWT (JSON Web Token)

🛠 Features
Multi-Company Support: Manage multiple companies with unique salary percentage configurations (Basic, House Rent, Medical).

Employee Management: Full CRUD operations for Employees, Departments, and Designations.

Attendance Tracking: Manage shifts and track employee In-Time, Out-Time, and Late-Time.

Automated Payroll: Calculate salaries based on attendance, including automatic deductions for absences using database procedures.

Reporting System: Generate filtered reports for Employee lists, Attendance summaries, and Salary sheets.

📁 Database Configuration
This project utilizes PostgreSQL for data persistence.

[!IMPORTANT]
Database Scripts: All SQL scripts required to set up the tables, relationships, and stored procedures are located in the dbScript folder of this repository. Please refer to these notes to initialize your database environment.

💻 Getting Started
Clone the Repository:

Bash
git clone https://github.com/your-username/your-repo-name.git
Database Setup:

Create a new PostgreSQL database.

Navigate to the dbScript folder.

Execute the scripts provided in the notes to create the schema and necessary functions.

Configuration:

Update the ConnectionStrings in the appsettings.json file with your PostgreSQL credentials.

Run the Project:

Open the solution in Visual Studio.

Build and Run (F5).

📄 License
This project is for educational and professional assessment purposes.

Author
A. N. Shuvo
Full-Stack Developer & UI/UX Designer
