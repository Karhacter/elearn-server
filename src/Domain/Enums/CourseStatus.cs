namespace elearn_server.Domain.Enums;

public enum CourseStatus
{
    Draft = 0,      // Bản nháp, chưa công khai
    PendingReview = 1, // Chờ duyệt trước khi xuất bản
    Published = 2,  // Đã xuất bản, người dùng có thể thấy và mua
    Archived = 3    // Đã lưu trữ, không còn bán nữa nhưng user cũ vẫn học được
}
