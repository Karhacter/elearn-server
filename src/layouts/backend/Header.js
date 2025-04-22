import { Link, useNavigate } from "react-router-dom";
import UserService from "../../services/UserService";

const Header = () => {
  const navigate = useNavigate();

  const HandleLogout = async () => {
    try {
      await UserService.logout();
      localStorage.removeItem("sessionToken");
      localStorage.removeItem("userRole");
      navigate("/");
    } catch (error) {
      console.error("Logout failed:", error);
    }
  };

  return (
    <>
      <section className="header bg-dark">
        <div className="container-fluid">
          <nav
            className="navbar navbar-expand-lg navbar-light bg-dark"
            data-bs-theme="dark"
          >
            <div className="container-fluid">
              <Link className="navbar-brand" to="/admin">
                Quản Trị
              </Link>
              <button
                className="navbar-toggler"
                type="button"
                data-bs-toggle="collapse"
                data-bs-target="#navbarSupportedContent"
                aria-controls="navbarSupportedContent"
                aria-expanded="false"
                aria-label="Toggle navigation"
              >
                <span className="navbar-toggler-icon"></span>
              </button>
              <div
                className="collapse navbar-collapse"
                id="navbarSupportedContent"
              >
                <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                  <li className="nav-item dropdown">
                    <Link
                      className="nav-link dropdown-toggle text-white"
                      to="#"
                      id="navbarDropdown"
                      role="button"
                      data-bs-toggle="dropdown"
                      aria-expanded="false"
                    >
                      Khóa Học
                    </Link>
                    <ul
                      className="dropdown-menu"
                      aria-labelledby="navbarDropdown"
                    >
                      <li>
                        <Link className="dropdown-item" to="/admin/course">
                          Tất cả Khóa Học
                        </Link>
                      </li>
                      <li>
                        <Link className="dropdown-item" to="/admin/category">
                          Danh mục Khóa Học
                        </Link>
                      </li>
                    </ul>
                  </li>

                  <li className="nav-item ">
                    <Link className="nav-link text-white" to="/admin/order">
                      Đơn hàng
                    </Link>
                  </li>

                  <li className="nav-item">
                    <Link className="nav-link text-white" to="/admin/order">
                      Chi Tiết Đơn Hàng
                    </Link>
                  </li>

                  <li className="nav-item">
                    <Link className="nav-link text-white" to="/admin/user">
                      Thành viên
                    </Link>
                  </li>
                </ul>
                <ul className="navbar-nav ms-auto mb-2 mb-lg-0 d-flex">
                <li className="nav-item">
                    <Link className="nav-link" to="/home">
                      Client
                    </Link>
                  </li>
                  <li className="nav-item">
                    <Link className="nav-link" onClick={HandleLogout} to="#">
                      Logout
                    </Link>
                  </li>
                </ul>
              </div>
            </div>
          </nav>
        </div>
      </section>
    </>
  );
};

export default Header;
