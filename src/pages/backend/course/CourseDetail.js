import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import CourseService from "../../../services/CourseService";

const CourseDetail = () => {
  const [course, setCourse] = useState({});
  let { id } = useParams();

  useEffect(() => {
    (async () => {
      const result = await CourseService.show(id);

      // If result.course is an array, convert it to an object
      setCourse(result);
      console.log(result);
    })();
  }, []);

  return (
    <div className="container">
      <section className="content-header my-2">
        <h1 className="d-inline">
          Chi tiết Khóa Học <mark>{course.title}</mark>
        </h1>
        <div className="row mt-2 align-items-center">
          <div className="col-md-12 text-end">
            <Link to={"/admin/course"} className="btn btn-primary btn-sm me-2">
              <i className="fa fa-arrow-left"></i> Về danh sách
            </Link>
            <Link
              to={`/admin/course/edit/${course.id}`}
              className="btn btn-success btn-sm"
            >
              <i className="fa fa-edit"></i> Sửa
            </Link>
          </div>
        </div>
      </section>
      <section className="content-body my-2">
        <table className="table table-bordered">
          <thead>
            <tr>
              <th>Tên trường</th>
              <th>Giá trị</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>ID</td>
              <td>{course.courseId}</td>
            </tr>
            <tr>
              <td>Tên</td>
              <td>{course.title}</td>
            </tr>

            <tr>
              <td>Mô tả</td>
              <td>{course.description}</td>
            </tr>

            <tr>
              <td>Thời lượng</td>
              <td>{course.duration} phút</td>
            </tr>
            <tr>
              <td>Giá</td>
              <td>{course.price} USD</td>
            </tr>
            <tr>
              <td>Giảm giá</td>
              <td>{course.discount} USD</td>
            </tr>
            <tr>
              <td>Thể loại (Genre ID)</td>
              <td>{course.genreId}</td>
            </tr>
            <tr>
              <td>Giảng viên (Instructor ID)</td>
              <td>{course.instructorId}</td>
            </tr>
            <tr>
              <td>Ảnh đại diện (Image)</td>
              <td>
                <img src={course.image} alt="Course" width="150" />
              </td>
            </tr>
            <tr>
              <td>Thumbnail</td>
              <td>
                <a
                  href={course.thumbnail}
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  {course.thumbnail}
                </a>
              </td>
            </tr>
            <tr>
              <td>Ngày tạo</td>
              <td>{new Date(course.createdAt).toLocaleString()}</td>
            </tr>
            <tr>
              <td>Ngày cập nhật</td>
              <td>{new Date(course.updatedAt).toLocaleString()}</td>
            </tr>
            <tr>
              <td>Cập nhật bởi</td>
              <td>{course.updatedBy}</td>
            </tr>
          </tbody>
        </table>
      </section>
    </div>
  );
};

export default CourseDetail;
