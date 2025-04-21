import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import UserService from "../../../services/UserService";

const UserDetail = () => {
  const [user, setUser] = useState({});
  let { id } = useParams();
  useEffect(() => {
    (async () => {
      const result = await UserService.show(id);

      const userObject = result;
      setUser(userObject);
      console.log(result);
    })();
  }, []);

  return (
    <div className="container">
      <section className="content-header my-2">
        <h1 className="d-inline">Chi tiết</h1>
        <div className="row mt-2 align-items-center">
          <div className="col-md-12 text-end">
            <Link to={"/admin/user"} className="btn btn-primary btn-sm me-2">
              <i className="fa fa-arrow-left"></i> Về danh sách
            </Link>
            <Link
              to={`/admin/user/edit/${user.userId}`}
              className="btn btn-success btn-sm"
            >
              <i className="fa fa-edit"></i> Sửa
            </Link>
          </div>
        </div>
      </section>
      <section className="content-body my-2">
        <table className="table table-bordered">
          <thead>
            <tr>
              <th>Tên trường</th>
              <th>Giá trị</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>ID</td>
              <td>{user.userId}</td>
            </tr>
            <tr>
              <td>Họ và tên</td>
              <td>{user.fullName}</td>
            </tr>
            <tr>
              <td>Email</td>
              <td>{user.email}</td>
            </tr>
            <tr>
              <td>Số điện thoại</td>
              <td>{user.phoneNumber}</td>
            </tr>
            <tr>
              <td>Vai trò</td>
              <td>{user.role}</td>
            </tr>
            <tr>
              <td>Ảnh đại diện</td>
              <td>
                <img src={user.profilePicture} alt="Profile" width="120" />
              </td>
            </tr>
          </tbody>
        </table>
      </section>
    </div>
  );
};

export default UserDetail;
