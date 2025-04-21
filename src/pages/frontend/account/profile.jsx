import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FaUserEdit } from "react-icons/fa";
const Profile = () => {
  const [userData, setUserData] = useState({});
  const navigate = useNavigate();

  useEffect(() => {
    const user = localStorage.getItem("userData");
    
    if (!user) {
      alert("Bạn chưa đăng nhập. Vui lòng đăng nhập lại.");
      navigate("/login");
    } else {
      const parsedUser = JSON.parse(user);
      console.log("Parsed User Data:", parsedUser); // Check the data structure
      setUserData(parsedUser);
    }
  }, [navigate]);
 


  return (
    <section className="profile-maincontent py-5">
      <div className="container">
        <div className="row justify-content-center">
          <div className="col-md-8">
            <div className="card shadow-lg border-0 rounded-4">
              <div className="card-body text-center">
                {/* User Avatar */}
                <div className="profile-avatar mb-3">
                  <img
                    src="https://avatars.githubusercontent.com/u/109360401?v=4"
                    alt="User Avatar"
                    className="rounded-circle shadow-lg border"
                    width="120"
                    height="120"
                  />
                </div>

                {/* User Name */}
                <h2 className="text-primary fw-bold">{userData.username || "Guest"}</h2>
                <p className="text-muted">{userData.email || "No email provided"}</p>
                

                {/* User Info Table */}
                <table className="table table-hover mt-3">
                  <tbody>
                    <tr>
                      <td className="fw-bold">Phone</td>
                      <td>{userData.phone || "N/A"}</td>
                    </tr>
                    <tr>
                      <td className="fw-bold">Address</td>
                      <td>
                        {userData.address || "N/A"}{" "}
                        <Link to="/edit-profile" className="text-primary ms-2">
                          <FaUserEdit />
                        </Link>
                      </td>
                    </tr>
                  </tbody>
                </table>

                {/* Action Buttons */}
                <div className="mt-4">
                  <Link to="/edit-profile" className="btn btn-outline-primary me-2">
                    Edit Profile
                  </Link>
                  <Link to="/home" className="btn btn-outline-secondary">
                    Back to Home
                  </Link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default Profile;