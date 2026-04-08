using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace elearn_server.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToCourseSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseRequirements_Course_CourseId",
                table: "CourseRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTargetAudiences_Course_CourseId",
                table: "CourseTargetAudiences");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningOutcomes_Course_CourseId",
                table: "LearningOutcomes");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CourseSections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseRequirements_Course_CourseId",
                table: "CourseRequirements",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTargetAudiences_Course_CourseId",
                table: "CourseTargetAudiences",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningOutcomes_Course_CourseId",
                table: "LearningOutcomes",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseRequirements_Course_CourseId",
                table: "CourseRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTargetAudiences_Course_CourseId",
                table: "CourseTargetAudiences");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningOutcomes_Course_CourseId",
                table: "LearningOutcomes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CourseSections");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseRequirements_Course_CourseId",
                table: "CourseRequirements",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTargetAudiences_Course_CourseId",
                table: "CourseTargetAudiences",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningOutcomes_Course_CourseId",
                table: "LearningOutcomes",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");
        }
    }
}
