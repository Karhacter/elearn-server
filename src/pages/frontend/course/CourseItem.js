import { useState } from "react";
import { urlImage } from "../../../config";
import { Link } from "react-router-dom";

const CourseItem = (props) => {
  const course = props.course;
  const [hovered, setHovered] = useState(false);

  return (
    <>
      <div className="single-case">
        <div className="case-img">
          <Link to={"/home/course/detail/" + course.courseId}>
            <img src={urlImage + course.image} alt={course.image} />
          </Link>
        </div>

        <div className="content">
          <Link to={"/home/course/detail/" + course.courseId}>
            <h3>{course.title}</h3>
          </Link>
          <p
            className={`course-description ${hovered ? "expanded" : ""}`}
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
            title={course.description}
          >
            {course.description}
          </p>

          <a href="case-details.html" className="line-bnt">
            View Project Details
          </a>
        </div>
      </div>
      <style>{`
        .course-description {
          white-space: nowrap;
          overflow: hidden;
          text-overflow: ellipsis;
          cursor: pointer;
          transition: all 0.3s ease;
          max-height: 2em; /* approx one line */
        }
        .course-description.expanded {
          white-space: normal;
          max-height: 100vh;
        }
      `}</style>
    </>
  );
};

export default CourseItem;
