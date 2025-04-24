import { useState } from "react";
import { Link } from "react-router-dom";
import UserService from "../../../services/UserService";

const UserCreate = () => {
  const [insertId, setInsertId] = useState(0);

  const [password, setPassword] = useState("");
  const [repassword, setRepassword] = useState("");
  const [email, setEmail] = useState("");
  const [PhoneNumber, setPhone] = useState("");
  const [fullName, setFullName] = useState("");
  const [role, setRole] = useState("Admin");
  const [imageFile, setImageFile] = useState(null);

  const [uploading, setUploading] = useState(false);
  const [message, setMessage] = useState("");
  const [errors, setErrors] = useState({});

  const handleImageChange = (e) => {
    if (e.target.files && e.target.files[0]) {
      setImageFile(e.target.files[0]);
    }
  };

  const validate = () => {
    const newErrors = {};

    if (password != repassword) {
      newErrors.title = "Mật Khẩu không trùng khớp";
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      const userData = {
        fullName,
        email,
        password,
        PhoneNumber,
        role,
      };

      if (!validate()) {
        return;
      }

      const createRes = await UserService.store(userData);
      const createdUser = createRes;

      if (imageFile) {
        setUploading(true);
        const formData = new FormData();
        formData.append("imageFile", imageFile);
        await UserService.uploadImage(createdUser.userId, formData);
        setUploading(false);
      }
      alert("Thêm Thành Viên Mới Thành Công");
      window.location.href = "/admin/user";
    } catch (error) {
      setUploading(false);
      setMessage(
        "Error creating course: " + (error.response?.data || error.message)
      );
    }
  };
  return (
    <div className="bg-light p-3">
      <section className="content-header my-2">
        <h3 className="d-inline">Thêm thành viên</h3>
        <form onSubmit={handleSubmit}>
          <div className="row mt-2 align-items-center">
            <div className="col-md-12 text-end">
              <button className="btn btn-success btn-sm">
                <i className="fa fa-save"></i> Lưu [Thêm]
              </button>
              <Link to={"/admin/user"} className="btn btn-primary btn-sm ms-2">
                <i className="fa fa-arrow-left"></i> Về danh sách
              </Link>
            </div>
          </div>
        </form>
      </section>
      <section className="content-body my-2">
        <form>
          <div className="row">
            <div className="col-md-6">
              <div className="mb-3">
                <label>
                  <strong>Email(*)</strong>
                </label>
                <input
                  type="text"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="form-control"
                  placeholder="Email"
                />
              </div>
              <div className="mb-3">
                <label>
                  <strong>Mật khẩu(*)</strong>
                </label>
                <input
                  type="password"
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  className="form-control"
                  placeholder="Mật khẩu"
                />
              </div>
              <div className="mb-3">
                <label>
                  <strong>Xác nhận mật khẩu(*)</strong>
                </label>
                <input
                  type="password"
                  value={repassword}
                  onChange={(event) => setRepassword(event.target.value)}
                  className="form-control"
                  placeholder="Xác nhận mật khẩu"
                />
              </div>

              <div className="mb-3">
                <label>
                  <strong>Điện thoại(*)</strong>
                </label>
                <input
                  type="text"
                  value={PhoneNumber}
                  onChange={(e) => setPhone(e.target.value)}
                  className="form-control"
                  placeholder="Điện thoại"
                />
              </div>
            </div>
            <div className="col-md-6">
              <div className="mb-3">
                <label>
                  <strong>Họ tên (*)</strong>
                </label>
                <input
                  type="text"
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                  className="form-control"
                  placeholder="Họ tên"
                />
              </div>

              <div className="mb-3">
                <label>
                  <strong>Hình đại diện</strong>
                </label>
                <input
                  type="file"
                  id="image"
                  className="form-control"
                  onChange={handleImageChange}
                />
                {imageFile && (
                  <img
                    src={URL.createObjectURL(imageFile)}
                    alt="Preview"
                    style={{
                      marginTop: "10px",
                      maxWidth: "100%",
                      maxHeight: "200px",
                    }}
                  />
                )}
              </div>
            </div>
          </div>
        </form>
      </section>
    </div>
  );
};

export default UserCreate;
