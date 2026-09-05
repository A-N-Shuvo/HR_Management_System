using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    ComId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComName = table.Column<string>(type: "text", nullable: false),
                    Basic = table.Column<decimal>(type: "numeric", nullable: false),
                    Hrent = table.Column<decimal>(type: "numeric", nullable: false),
                    Medical = table.Column<decimal>(type: "numeric", nullable: false),
                    IsInactive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.ComId);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DeptId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeptName = table.Column<string>(type: "text", nullable: false),
                    ComId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DeptId);
                    table.ForeignKey(
                        name: "FK_Department_Company_ComId",
                        column: x => x.ComId,
                        principalTable: "Company",
                        principalColumn: "ComId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Designation",
                columns: table => new
                {
                    DesigId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesigName = table.Column<string>(type: "text", nullable: false),
                    ComId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designation", x => x.DesigId);
                    table.ForeignKey(
                        name: "FK_Designation_Company_ComId",
                        column: x => x.ComId,
                        principalTable: "Company",
                        principalColumn: "ComId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftName = table.Column<string>(type: "text", nullable: false),
                    InTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    OutTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    LateTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ComId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shift", x => x.ShiftId);
                    table.ForeignKey(
                        name: "FK_Shift_Company_ComId",
                        column: x => x.ComId,
                        principalTable: "Company",
                        principalColumn: "ComId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmpId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpCode = table.Column<string>(type: "text", nullable: false),
                    EmpName = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    Gross = table.Column<decimal>(type: "numeric", nullable: false),
                    Basic = table.Column<decimal>(type: "numeric", nullable: false),
                    HRent = table.Column<decimal>(type: "numeric", nullable: false),
                    Medical = table.Column<decimal>(type: "numeric", nullable: false),
                    Others = table.Column<decimal>(type: "numeric", nullable: false),
                    dtJoin = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ComId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeptId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmpId);
                    table.ForeignKey(
                        name: "FK_Employee_Company_ComId",
                        column: x => x.ComId,
                        principalTable: "Company",
                        principalColumn: "ComId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employee_Department_DeptId",
                        column: x => x.DeptId,
                        principalTable: "Department",
                        principalColumn: "DeptId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employee_Designation_DesigId",
                        column: x => x.DesigId,
                        principalTable: "Designation",
                        principalColumn: "DesigId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employee_Shift_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shift",
                        principalColumn: "ShiftId");
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    dtDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AttStatus = table.Column<string>(type: "text", nullable: true),
                    InTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    OutTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ComId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendance_Company_ComId",
                        column: x => x.ComId,
                        principalTable: "Company",
                        principalColumn: "ComId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendance_Employee_EmpId",
                        column: x => x.EmpId,
                        principalTable: "Employee",
                        principalColumn: "EmpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceSummary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    dtYear = table.Column<int>(type: "integer", nullable: false),
                    dtMonth = table.Column<int>(type: "integer", nullable: false),
                    Present = table.Column<int>(type: "integer", nullable: false),
                    Late = table.Column<int>(type: "integer", nullable: false),
                    Absent = table.Column<int>(type: "integer", nullable: false),
                    ComId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceSummary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceSummary_Company_ComId",
                        column: x => x.ComId,
                        principalTable: "Company",
                        principalColumn: "ComId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceSummary_Employee_EmpId",
                        column: x => x.EmpId,
                        principalTable: "Employee",
                        principalColumn: "EmpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Salary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    dtYear = table.Column<int>(type: "integer", nullable: false),
                    dtMonth = table.Column<int>(type: "integer", nullable: false),
                    Gross = table.Column<decimal>(type: "numeric", nullable: false),
                    Basic = table.Column<decimal>(type: "numeric", nullable: false),
                    Hrent = table.Column<decimal>(type: "numeric", nullable: false),
                    Medical = table.Column<decimal>(type: "numeric", nullable: false),
                    AbsentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PayableAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ComId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salary_Company_ComId",
                        column: x => x.ComId,
                        principalTable: "Company",
                        principalColumn: "ComId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Salary_Employee_EmpId",
                        column: x => x.EmpId,
                        principalTable: "Employee",
                        principalColumn: "EmpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_ComId",
                table: "Attendance",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_EmpId",
                table: "Attendance",
                column: "EmpId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSummary_ComId",
                table: "AttendanceSummary",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSummary_EmpId",
                table: "AttendanceSummary",
                column: "EmpId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_ComId",
                table: "Department",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_Designation_ComId",
                table: "Designation",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ComId",
                table: "Employee",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DeptId",
                table: "Employee",
                column: "DeptId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_DesigId",
                table: "Employee",
                column: "DesigId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_ShiftId",
                table: "Employee",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Salary_ComId",
                table: "Salary",
                column: "ComId");

            migrationBuilder.CreateIndex(
                name: "IX_Salary_EmpId",
                table: "Salary",
                column: "EmpId");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_ComId",
                table: "Shift",
                column: "ComId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "AttendanceSummary");

            migrationBuilder.DropTable(
                name: "Salary");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Designation");

            migrationBuilder.DropTable(
                name: "Shift");

            migrationBuilder.DropTable(
                name: "Company");
        }
    }
}
