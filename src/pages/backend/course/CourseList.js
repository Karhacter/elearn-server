import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import CourseService from "../../../services/CourseService";
import CategoryService from "../../../services/CategoryService";
import { urlImage } from "../../../config";

const CourseList = () => {
  const [insertId, setInsertId] = useState(0);
  //
  const [courses, setCourses] = useState([]);
  const [categorys, setCategorys] = useState([]);

  // Fetch courses
  useEffect(() => {
    (async () => {
      const result = await CourseService.get_list();
      console.log("Fetched courses:", result);
      setCourses(result); // Use $values array if present
    })();
  }, [insertId]);

  // Fetch categories
  useEffect(() => {
    (async () => {
      const result = await CategoryService.get_list();
      console.log("Fetched categories:", result);
      setCategorys(result);
    })();
  }, []);

  const handleDelete = async (courseId) => {
    const result = await CourseService.remove(courseId);

    alert("Xóa Thành Công");
    setInsertId(result.insertId);
  };

  // Helper to get category name by genreId
  const getCategoryName = (genreId) => {
    const category = categorys.find((cat) => cat.id === genreId);
    return category ? category.name : genreId;
  };

  return (
    <div className="card">
      <div className="card-header">
        <div className="row">
          <div className="col-6">
            <strong className="text-danger">Tất cả Khóa Học</strong>
          </div>
          <div className="col-6 text-end">
            <Link to="/admin/course/create" className="btn btn-sm btn-success">
              Thêm Khóa Học
            </Link>
          </div>
        </div>
      </div>
      <div className="card-body">
        <table className="table table-bordered table-hover table-striped">
          <thead>
            <tr>
              <th scope="col">#</th>
              <th scope="col">Hinh</th>
              <th scope="col">Ten Khóa Học</th>
              <th scope="col">Thể loại</th>
              <th scope="col">Hành động</th>
            </tr>
          </thead>
          <tbody>
            {courses.length > 0 &&
              courses.map((course, index) => {
                return (
                  <tr key={index}>
                    <th scope="row">{course.courseId}</th>
                    <td>
                      <img
                        src={urlImage + course.image}
                        style={{ width: "80px" }}
                      />
                    </td>
                    <td>
                      <Link to={"/admin/course/detail/" + course.courseId}>
                        {course.title}
                      </Link>
                    </td>
                    <td>{getCategoryName(course.genreId)}</td>
                    <td>
                      <Link
                        className="px-1 me-1 text-primary"
                        to={"/admin/course/edit/" + course.courseId}
                      >
                        <i className="btn btn btn-primary fa fa-edit"></i>
                      </Link>
                      <Link className="px-1 me-1 text-danger">
                        <i
                          className="btn btn btn-danger fa fa-trash"
                          onClick={() => handleDelete(course.courseId)}
                        ></i>
                      </Link>
                    </td>
                  </tr>
                );
              })}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default CourseList;
