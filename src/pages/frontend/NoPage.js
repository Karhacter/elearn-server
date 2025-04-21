import { Link } from "react-router-dom";
import image from "../../asset/images/404.gif";

const NoPage = () => {
  return (
    <div className="error-area">
      <div className="d-table">
        <div className="d-table-cell">
          <div className="error-page">
            <img src={image} alt="error" />
            <h3>Oops! Page Not Found</h3>
            <p>The page you were looking for could not be found.</p>
            <Link to="/home" className="box-btn">
              Return To Home Page
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default NoPage;
