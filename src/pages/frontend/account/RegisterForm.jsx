import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import UserService from "../../../services/UserService";

const RegisterForm = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [repassword, setRepassword] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [name, setName] = useState("");
  const [gender, setGender] = useState(1);
  const [address, setAddress] = useState("");
  const [errors, setErrors] = useState({});
  const navigate = useNavigate();
  const [insertId, setInsertId] = useState(0);
  const [status, setStatus] = useState(2);

  const handleSubmit = async (event) => {
    event.preventDefault();

    const image = document.querySelector("#image");

    // Validate input fields
    const newErrors = {};
    if (!username) newErrors.username = "Username is required.";
    if (!email) newErrors.email = "Email is required.";
    if (!password) newErrors.password = "Password is required.";
    if (!repassword) newErrors.repassword = "Password confirmation is required.";

    if (Object.keys(newErrors).length > 0) {
        setErrors(newErrors);
        return;
    }

    const isPasswordComplex = (password) => {
        const minLength = 8;
        const hasUpperCase = /[A-Z]/.test(password);
        const hasLowerCase = /[a-z]/.test(password);
        const hasNumbers = /\d/.test(password);
        const hasSpecialChars = /[!@#$%^&*(),.?":{}|<>]/.test(password);
        return (
            password.length >= minLength &&
            hasUpperCase &&
            hasLowerCase &&
            hasNumbers &&
            hasSpecialChars
        );
      };

        if (!isPasswordComplex(password)) {
            alert("Passwords must be at least 8 characters, including uppercase, lowercase, numbers, and special characters.");
            return;
        }

        if (repassword !== password) {
            alert("The confirmation password is incorrect.");
            return;
        }




        // Create a form data object
        const user = new FormData();
        user.append("username", username);
        user.append("password", password);
        user.append("repassword", repassword);
        user.append("email", email);
        user.append("phone", phone);
        user.append("name", name);
        user.append("gender", gender);
        user.append("address", address);
        user.append("status", status);
        user.append("image", image.files.length === 0 ? "" : image.files[0]);

        console.log("Dữ liệu người dùng:", user);
        // Submit form using UserService
        (async () => {
            const result = await UserService.store(user);
            console.log("Kết quả đăng ký:", result);
        
            if (result.status === true) {
                alert(result.message);
                setInsertId(result.user.insertId);
                navigate("/login");
            } else {
                setErrors({ general: result.message });
            }
        })();
    };

  return (
    <div className="d-flex vh-100 align-items-center justify-content-center bg-light">
      <div className="card shadow-lg p-4" style={{ width: "24rem" }}>
        <h2 className="text-center text-primary mb-3">Đăng Ký</h2>
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label htmlFor="name" className="form-label">
              Name
            </label>
            <input
              type="text"
              id="username"
              name="username"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              className="form-control"
              placeholder="Enter your name"
              required
            />
          </div>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">
              Email
            </label>
            <input
              type="email"
              id="email"

              value={email}
              onChange={(event) => setEmail(event.target.value)}
              className="form-control"
              placeholder="Enter your email"
              required
            />
          </div>
          <div className="mb-3">
            <label htmlFor="password" className="form-label">
              Password
            </label>
            <input
              type="password"
              id="password"
              
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              className="form-control"
              placeholder="Enter your password"
              required
            />
          </div>
          <button type="submit" className="btn btn-primary w-100">
            Register
          </button>
          <p className="text-center text-muted mt-3">
    
          <Link to="/home" className="text-decoration-none">
            Continue as Guest
          </Link>
        </p>
        </form>
        <p className="text-center text-muted mt-3">
          Already have an account?{" "}
          <Link to="/login" className="text-decoration-none">
            Login
          </Link>
        </p>
      </div>
    </div>
  );
};

export default RegisterForm;