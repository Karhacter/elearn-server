import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import CategoryService from "../../services/CategoryService";

const Header = () => {
  const [categorys, setCategorys] = useState([]);
  const nagivate = useNavigate();
  useEffect(() => {
    (async () => {
      const res = await CategoryService.get_list();
      console.log("fetch: ", res);
      setCategorys(res);
    })();
  }, []);

  return (
    <>
      <header className="header-area header-2">
        <div className="container">
          <div className="row align-items-center">
            <div className="col-lg-8 col-md-7 text-left">
              <div className="header-content-right">
                <ul className="header-contact">
                  <li>
                    <Link to="tel:+1123456789">
                      <i className="bx bxs-phone-call"></i> +0378 173 109
                    </Link>
                  </li>
                  <li>
                    <Link to="mailto:hello@paso.com">
                      <i className="bx bxs-envelope"></i> khanhduc392@gmail.com
                    </Link>
                  </li>
                </ul>
              </div>
            </div>
            <div className="col-lg-4 col-md-5 text-right">
              <div className="header-content-right">
                <ul className="header-social">
                  <li>
                    <Link to="#" target="_blank">
                      <i className="bx bxl-facebook"></i>
                    </Link>
                  </li>
                  <li>
                    <Link to="#" target="_blank">
                      <i className="bx bxl-twitter"></i>
                    </Link>
                  </li>
                  <li>
                    <Link to="#" target="_blank">
                      {" "}
                      <i className="bx bxs-envelope"></i>
                    </Link>
                  </li>
                  <li>
                    <Link to="#" target="_blank">
                      {" "}
                      <i className="bx bxl-google-plus"></i>
                    </Link>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </header>

      <div className="navbar-area">
        <div className="mobile-nav">
          <Link to="index.html" className="logo">
            <img src="assets\images\logo.png" alt="logo" />
          </Link>
        </div>

        <div className="main-nav">
          <div className="container">
            <nav className="navbar navbar-expand-md navbar-light">
              <div
                className="collapse navbar-collapse mean-menu"
                id="navbarSupportedContent"
              >
                <ul className="navbar-nav text-center">
                  <li className="nav-item">
                    <Link to="/home" className="nav-link active">
                      Trang Chủ
                    </Link>
                  </li>
                  <li className="nav-item">
                    <Link to="/about" className="nav-link">
                      Giới Thiệu
                    </Link>
                  </li>
                  <li className="nav-item">
                    <Link to="#" className="nav-link ">
                      Giải Pháp
                    </Link>
                  </li>
                  <li className="nav-item">
                    <Link
                      to="/home/course"
                      className="nav-link dropdown-toggle"
                    >
                      Khóa Học
                    </Link>
                    <ul className="dropdown-menu">
                      {categorys.length > 0 &&
                        categorys.map((category) => (
                          <li className="nav-item">
                            <Link
                              to={"/home/course/category/" + category.id}
                              className="nav-link"
                            >
                              {category.name}
                            </Link>
                          </li>
                        ))}
                    </ul>
                  </li>
                  <li className="nav-item">
                    <Link to="#" className="nav-link">
                      Blog
                    </Link>
                  </li>
                  <li className="nav-item">
                    <Link to="/home/cart" className="nav-link ">
                      Giỏ Hàng
                    </Link>
                  </li>
                  <li className="nav-item">
                    <Link to="/home/contact" className="nav-link">
                      Liên Hệ
                    </Link>
                  </li>
                </ul>
              </div>
              <div className="nav-right">
                <form>
                  <div className="input-group">
                    <input
                      type="text"
                      className="form-control search"
                      placeholder="Search..."
                    />
                  </div>
                  <button type="submit">
                    <i className="bx bx-search"></i>
                  </button>
                </form>
              </div>
              <div className="nav-btn">
                <Link to="/register" className="box-btn">
                  Get Started
                </Link>
              </div>
            </nav>
          </div>
        </div>
      </div>
    </>
  );
};

export default Header;
