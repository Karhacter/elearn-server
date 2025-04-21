import Image3 from "../../../asset/images/shape/3.png";
import Image4 from "../../../asset/images/shape/4.png";
import Image5 from "../../../asset/images/shape/5.png";
import Image6 from "../../../asset/images/shape/6.png";
import CourseItem from "./CourseItem";
import CourseService from "../../../services/CourseService";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

const Course = () => {
  const [courses, setCourses] = useState([]);
  // Fetch courses
  useEffect(() => {
    (async () => {
      const result = await CourseService.get_list();
      console.log("Fetched courses:", result);
      setCourses(result); // Use $values array if present
    })();
  }, []);
  return (
    <>
      <div className="page-title-area">
        <div className="container">
          <div className="page-title-content">
            <h2>Case Studies</h2>
            <ul>
              <li>
                <a href="index.html">Trang Chủ</a>
              </li>

              <li className="active">Khóa Học</li>
            </ul>
          </div>
        </div>
        <div className="page-shape">
          <div className="shape1">
            <img src={Image4} alt="shape" />
          </div>
          <div className="shape3">
            <img src={Image3} alt="shape" />
          </div>

          <div className="shape5">
            <img src={Image5} alt="shape" />
          </div>
          <div className="shape6">
            <img src={Image6} alt="shape" />
          </div>
        </div>
      </div>

      <section className="home-case pt-100 pb-70">
        <div className="container">
          <div className="section-title">
            <h2>Tổng Quan Các Khóa Học Chúng Tôi Có</h2>
          </div>

          <div className="case">
            <ul className="all-case">
              <li className="active" data-filter="*">
                <span>All</span>
              </li>
              <li data-filter=".design">
                <span>Design</span>
              </li>
              <li data-filter=".dev">
                <span>Development</span>
              </li>
              <li data-filter=".cyber">
                <span>Cyber Security</span>
              </li>
            </ul>
          </div>

          <div className="row case-list">
            {courses.length > 0 ? (
              courses.map((course) => (
                <div
                  className="col-lg-4 col-sm-6 item cyber"
                  key={course.courseId}
                >
                  <CourseItem course={course} />
                </div>
              ))
            ) : (
              <p className="text-center">Không có Khóa học nào.</p>
            )}
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

export default Course;
