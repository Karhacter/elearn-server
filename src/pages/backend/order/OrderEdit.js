import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import OrderService from "../../../services/OrderService";
import UserService from "../../../services/UserService";

const OrderEdit = () => {
  const [order, setOrder] = useState([]);
  const [users, setUsers] = useState([]);
  let { id } = useParams();

  useEffect(function () {
    (async () => {
      const result = await UserService.get_list();

      setUsers(result);
      console.log(result);
    })();
  }, []);
  const [name, setName] = useState("");
  const [phone, setPhone] = useState("");
  const [email, setEmail] = useState("");
  const [address, setAddress] = useState("");
  const [userId, setUserId] = useState(null);
  const [statusOrderId, setStatusOrderId] = useState(null);
  const [orderDetails, setOrderDetails] = useState([]);

  // message
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    (async () => {
      const result = await OrderService.orderDetail(id);

      // Assuming result.product contains the course object
      const orderObject = result.order || result;
      setOrder(orderObject);
      console.log(orderObject);
      setName(orderObject.name || "");
      setPhone(orderObject.phone || "");
      setEmail(orderObject.email || "");
      setAddress(orderObject.address || "");
      setUserId(orderObject.user_id ?? null);
      setStatusOrderId(orderObject.statusOrderId ?? null);
      setOrderDetails(orderObject.orderDetails || []);
    })();
  }, []);

  const handleOrderDetailCourseIdChange = (index, e) => {
    const newOrderDetails = [...orderDetails];
    newOrderDetails[index].courseID = parseInt(e.target.value) || 0;
    setOrderDetails(newOrderDetails);
  };

  const addOrderDetail = () => {
    setOrderDetails([...orderDetails, { courseID: 1, quantity: 1 }]);
  };

  const handleUpdate = async (e) => {
    e.preventDefault();
    setMessage("");

    // Validation: ensure no courseID is 0 in orderDetails
    for (const detail of orderDetails) {
      if (!detail.courseID || detail.courseID === 0) {
        setMessage("Course ID in order details cannot be 0 or empty.");
        return;
      }
    }

    try {
      const orderData = {
        name,
        phone,
        email,
        address,
        user_id: userId,
        StatusOrder: statusOrderId,
        orderDetails: orderDetails,
      };

      const createResponse = await OrderService.update(id, orderData);
      alert("Cập Nhật Đơn Hàng Thành Công");
      window.location.href = "/admin/order";
    } catch (error) {
      setLoading(false);
      setMessage(
        "Error creating course: " + (error.response?.data || error.message)
      );
    }
  };
  return (
    <form onSubmit={handleUpdate} className="container">
      <div className="card">
        <div className="card-header">
          <div className="row">
            <div className="col-6">
              <strong className="text-danger">Cập nhật Đơn Hàng</strong>
            </div>
            <div className="col-6 text-end">
              <button className="btn btn-sm btn-info me-2">
                <Link to="/admin/order" className="text-decoration-none">
                  Trở Về
                </Link>
              </button>
              <button className="btn btn-sm btn-success" type="submit">
                Cập nhật
              </button>
            </div>
          </div>
        </div>

        <div className="card-body">
          <input name="orderID" value={order.orderID} type="hidden" />

          <div className="mb-3">
            <label>
              <strong>Tên khách hàng</strong>
            </label>
            <input
              value={name}
              onChange={(e) => setName(e.target.value)}
              type="text"
              className="form-control"
            />
          </div>

          <div className="mb-3">
            <label>
              <strong>Số điện thoại</strong>
            </label>
            <input
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
              type="text"
              className="form-control"
            />
          </div>

          <div className="mb-3">
            <label>
              <strong>Email</strong>
            </label>
            <input
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              type="email"
              className="form-control"
            />
          </div>

          <div className="mb-3">
            <label>
              <strong>Địa chỉ</strong>
            </label>
            <input
              value={address}
              onChange={(e) => setAddress(e.target.value)}
              type="text"
              className="form-control"
            />
          </div>

          <div className="mb-3">
            <label>
              <strong>Trạng thái đơn hàng</strong>
            </label>
            <select
              value={statusOrderId}
              onChange={(e) => setStatusOrderId(parseInt(e.target.value))}
              className="form-control"
            >
              <option value="0">Chọn trạng thái</option>
              <option value="1">Pending</option>{" "}
              <option value="2">Complete</option>
            </select>
          </div>

          <div className="mb-3">
            <label>
              <strong>Người dùng</strong>
            </label>
            <select
              value={userId}
              onChange={(e) => setUserId(parseInt(e.target.value))}
              className="form-control"
            >
              <option value="0">Chọn người dùng</option>
              {users &&
                users.map((user) => (
                  <option value={user.userId} key={user.userId}>
                    {user.fullName}
                  </option>
                ))}
            </select>
          </div>

          {/* New UI for orderDetails */}
          <div className="mb-3">
            <label>
              <strong>Order Details</strong>
            </label>
            {orderDetails.map((detail, index) => (
              <div key={index} className="d-flex mb-2">
                <input
                  type="number"
                  className="form-control me-2"
                  value={detail.courseID}
                  onChange={(e) => handleOrderDetailCourseIdChange(index, e)}
                />
              </div>
            ))}
            <button
              type="button"
              className="btn btn-sm btn-primary"
              onClick={addOrderDetail}
            >
              Add Order Detail
            </button>
          </div>
        </div>
      </div>
    </form>
  );
};

export default OrderEdit;
