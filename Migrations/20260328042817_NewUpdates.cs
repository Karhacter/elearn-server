using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace elearn_server.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Courses_CourseId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Courses_CourseID",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Courses_CourseId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseRequirements_Courses_CourseId",
                table: "CourseRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Categories_GenreId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_InstructorId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseSections_Courses_CourseId",
                table: "CourseSections");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTargetAudiences_Courses_CourseId",
                table: "CourseTargetAudiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningOutcomes_Courses_CourseId",
                table: "LearningOutcomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Courses_CourseId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Courses_CourseId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Courses_CourseId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Courses_CourseId",
                table: "Wishlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "Courses",
                newName: "Course");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_Slug",
                table: "Course",
                newName: "IX_Course_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_InstructorId",
                table: "Course",
                newName: "IX_Course_InstructorId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_GenreId",
                table: "Course",
                newName: "IX_Course_GenreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Course",
                table: "Course",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Course_CourseId",
                table: "Assignments",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Course_CourseID",
                table: "CartItems",
                column: "CourseID",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Course_CourseId",
                table: "Certificates",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Course_CourseId",
                table: "Comments",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Categories_GenreId",
                table: "Course",
                column: "GenreId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Users_InstructorId",
                table: "Course",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseRequirements_Course_CourseId",
                table: "CourseRequirements",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSections_Course_CourseId",
                table: "CourseSections",
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
                name: "FK_Enrollments_Course_CourseId",
                table: "Enrollments",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningOutcomes_Course_CourseId",
                table: "LearningOutcomes",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Course_CourseId",
                table: "Lessons",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Course_CourseId",
                table: "OrderDetails",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Course_CourseId",
                table: "Payments",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Course_CourseId",
                table: "Quizzes",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Course_CourseId",
                table: "Ratings",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Course_CourseId",
                table: "Wishlists",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Course_CourseId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Course_CourseID",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Course_CourseId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Course_CourseId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_Categories_GenreId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_Users_InstructorId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseRequirements_Course_CourseId",
                table: "CourseRequirements");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseSections_Course_CourseId",
                table: "CourseSections");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTargetAudiences_Course_CourseId",
                table: "CourseTargetAudiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Course_CourseId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningOutcomes_Course_CourseId",
                table: "LearningOutcomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Course_CourseId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Course_CourseId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Course_CourseId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Course_CourseId",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Course_CourseId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Course_CourseId",
                table: "Wishlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Course",
                table: "Course");

            migrationBuilder.RenameTable(
                name: "Course",
                newName: "Courses");

            migrationBuilder.RenameIndex(
                name: "IX_Course_Slug",
                table: "Courses",
                newName: "IX_Courses_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Course_InstructorId",
                table: "Courses",
                newName: "IX_Courses_InstructorId");

            migrationBuilder.RenameIndex(
                name: "IX_Course_GenreId",
                table: "Courses",
                newName: "IX_Courses_GenreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Courses_CourseId",
                table: "Assignments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Courses_CourseID",
                table: "CartItems",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Courses_CourseId",
                table: "Certificates",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Courses_CourseId",
                table: "Comments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseRequirements_Courses_CourseId",
                table: "CourseRequirements",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Categories_GenreId",
                table: "Courses",
                column: "GenreId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_InstructorId",
                table: "Courses",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSections_Courses_CourseId",
                table: "CourseSections",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTargetAudiences_Courses_CourseId",
                table: "CourseTargetAudiences",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningOutcomes_Courses_CourseId",
                table: "LearningOutcomes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Courses_CourseId",
                table: "Lessons",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Courses_CourseId",
                table: "OrderDetails",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Courses_CourseId",
                table: "Payments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Courses_CourseId",
                table: "Quizzes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Courses_CourseId",
                table: "Ratings",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Courses_CourseId",
                table: "Wishlists",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId");
        }
    }
}
