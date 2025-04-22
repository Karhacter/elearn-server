import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import CourseService from "../../../services/CourseService";
import { urlImage } from "../../../config";
import CartService from "../../../services/CartService";
import UserService from "../../../services/UserService";

import Image3 from "../../../asset/images/shape/3.png";
import Image4 from "../../../asset/images/shape/4.png";
import Image5 from "../../../asset/images/shape/5.png";
import Image6 from "../../../asset/images/shape/6.png";
import Image from "../../../asset/images/case-details/1.jpg";
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

  const convertTime = (durationInMinutes) => {
    const convertedHourElement = document.getElementById("convertedHour");
    const hours = Math.floor(durationInMinutes / 60);
    const minutes = durationInMinutes % 60;
    convertedHourElement.innerHTML = `<i class="fa-regular fa-clock"></i> ${hours}h ${minutes}m`;
  };

  const handleAddToCart = async () => {
    const storedUserId = localStorage.getItem("sessionToken");
    if (!storedUserId) {
      alert("Bạn cần đăng nhập để thêm vào giỏ hàng!");
      window.location.href = "/";
      return;
    }

    try {
      const response = await UserService.checkAuth();
      if (response) {
        const userId = response.userId; // just use local variable
        const res = await CartService.addToCart(userId, course.courseId);
        if (res) {
          alert("✅ Đã thêm vào giỏ hàng!");
        }
      }
    } catch (err) {
      console.error("❌ Lỗi khi thêm vào giỏ hàng:", err);
      alert("❌ Không thể thêm vào giỏ hàng");
    }
  };

  return (
    <>
      <div className="page-title-area">
        <div className="container">
          <div className="page-title-content">
            <h2>Case: {course.title}</h2>
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

      <section className="services-details-area ptb-100">
        <div className="container">
          <div className="row">
            <div className="col-12">
              <div className="services-img mb">
                <img src={urlImage + course.image} alt="Image" />
              </div>
            </div>
            <div className="col-12">
              <div className="services-details-text">
                <h2>{course.title}</h2>
                <p
                  style={{
                    whiteSpace: "pre-wrap",
                    wordBreak: "break-word",
                  }}
                >
                  {course.description}
                </p>
              </div>
            </div>
          </div>

          <div className="scrives-item-2 mt-4 ">
            <div className="row align-items-center">
              <div className="col-lg-4">
                <div className="card">
                  <img
                    className="card-img-top"
                    src={Image}
                    alt="Card image cap"
                  />

                  <div className="card-body">
                    <Link
                      href="#"
                      className="btn btn-primary"
                      onClick={() => handleAddToCart(course.id)}
                    >
                      Thêm Vào
                    </Link>
                    <Link href="#" className="btn btn-danger ms-2">
                      Mua Luôn
                    </Link>
                  </div>
                </div>
              </div>

              <div className="col-lg-8">
                <div className="card">
                  <div class="card-body">
                    {/* Infor course */}
                    <div className="row">
                      <h2>Thông Tin Khóa Học</h2>
                      <div className="col">
                        <p className="card-text">
                          <i className="fa-regular fa-clock"></i>{" "}
                          {course.duration
                            ? `${Math.floor(course.duration / 60)}h ${course.duration % 60}m`
                            : "Updating..."}{" "}
                          học
                        </p>
                      </div>
                      <div className="col">
                        <p className="card-text">
                          <i class="fa-solid fa-money-bill"></i> {course.price}{" "}
                          USD
                        </p>
                      </div>
                    </div>
                    <div className="row pt-3">
                      <div className="col">
                        <p className="card-text">
                          <i className="fa-regular fa-clock"></i>{" "}
                          {course.duration
                            ? `${Math.floor(course.duration / 60)}h ${course.duration % 60}m`
                            : "Updating..."}{" "}
                          học
                        </p>
                      </div>
                      <div className="col">
                        <p className="card-text">
                          <i className="fa-regular fa-clock"></i>{" "}
                          {course.duration
                            ? `${Math.floor(course.duration / 60)}h ${course.duration % 60}m`
                            : "Updating..."}{" "}
                          học
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="scrives-item-3 mt-4">
            <div className="row align-items-center">
              <div className="col-lg-6">
                <div className="social">
                  <ul className="social-link">
                    <li>
                      <a href="#">
                        <i className="bx bxl-facebook"></i>
                      </a>
                    </li>
                    <li>
                      <a href="#">
                        <i className="bx bxl-twitter"></i>
                      </a>
                    </li>
                    <li>
                      <a href="#">
                        <i className="bx bxl-pinterest-alt"></i>
                      </a>
                    </li>
                    <li>
                      <a href="#">
                        <i className="bx bxl-instagram"></i>
                      </a>
                    </li>
                  </ul>
                </div>
              </div>

              <div className="col-lg-6">
                <div className="share">
                  <a href="#">
                    <i className="bx bx-share-alt"></i>
                    Share
                  </a>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};

export default CourseDetail;
