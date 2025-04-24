import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import UserService from "../../../services/UserService";

const UserEdit = () => {
  const [insertId, setInsertId] = useState(0);
  const [user, setUser] = useState([]);
  let { id } = useParams();

  useEffect(function () {
    (async () => {
      const result = await UserService.show(id);

      const userObject = result;

      setUser(userObject);
      console.log(result.user);
      setFullName(userObject.fullName);
      setPassword(userObject.password);
      setEmail(userObject.email);
      setPhone(userObject.phoneNumber);
      setRole(userObject.role);
    })();
  }, []);

  const [password, setPassword] = useState("");
  const [repassword, setRepassword] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhone] = useState("");
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

  const handleUpdate = async (event) => {
    event.preventDefault();

    try {
      const userData = {
        fullName,
        email,
        password,
        phoneNumber,
        role,
      };

      const createRes = await UserService.edit(id, userData);
      const createdUser = createRes;
      if (imageFile) {
        setUploading(true);
        const formData = new FormData();
        formData.append("imageFile", imageFile);
        await UserService.uploadImage(id, formData);
        setUploading(false);
      }
      alert("Cập Nhật Thông tin Thành Công");
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
        <form onSubmit={handleUpdate}>
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
                  <strong>Tên đăng nhập(*)</strong>
                </label>
                <input
                  type="text"
                  value={fullName}
                  onChange={(event) => setFullName(event.target.value)}
                  className="form-control"
                  placeholder="Tên đăng nhập"
                />
              </div>

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
                  <strong>Điện thoại(*)</strong>
                </label>
                <input
                  type="text"
                  value={phoneNumber}
                  onChange={(e) => setPhone(e.target.value)}
                  className="form-control"
                  placeholder="Điện thoại"
                />
              </div>
            </div>
            <div className="col-md-6">
              <div className="mb-3">
                <label>
                  <strong>Quyền Hạn</strong>
                </label>
                <select
                  value={role}
                  onChange={(e) => setRole(e.target.value)}
                  name="gender"
                  id="gender"
                  className="form-select"
                >
                  <option>Cấp Quyền</option>
                  <option value="Admin">Admin</option>
                  <option value="Instructor">Instructor</option>
                </select>
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

export default UserEdit;
