import CourseItem from "./CourseItem";
import CourseService from "../../../services/CourseService";
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";

const CourseCategory = () => {
  const [loading, setLoading] = useState(true);
  const [courses, setCourses] = useState([]);
  // Fetch courses
  let { id } = useParams();
  useEffect(() => {
    (async () => {
      const result = await CourseService.listCourseCategory(id);
      if (result) {
        console.log("Fetched courses:", result);
        setCourses(result);
      } else {
        setLoading(false);
      }
    })();
  }, [loading]);
  return (
    <>
      <div className="page-title-area">
        <h1 className="text-white">Our Courses Follow Category</h1>
      </div>

      <section className="home-case pt-100 pb-70">
        <div className="container">
          <div className="row case-list">
            {courses.length > 0 &&
              courses.map((course) => (
                <div
                  className="col-lg-4 col-sm-6 item cyber"
                  key={course.courseId}
                >
                  <CourseItem course={course} />
                </div>
              ))}
          </div>

          <div className="case-btn text-center">
            <p>
              We Have More Amazing Cases.{" "}
              <Link to="/home/course">View More</Link>
            </p>
          </div>
        </div>
      </section>
    </>
  );
};

export default CourseCategory;
