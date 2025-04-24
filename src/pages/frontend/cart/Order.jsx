import { useEffect, useState } from "react";
import OrderService from "../../../services/OrderService";
import UserService from "../../../services/UserService";
import { Link } from "react-router-dom";

const Order = () => {
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    (async () => {
      const sessionToken = localStorage.getItem("sessionToken");
      if (!sessionToken) {
        alert("Bạn cần Đăng Nhập Để Thanh Toán");
        window.location.href = "/";
        return;
      }

      try {
        const response = await UserService.checkAuth();
        if (response) {
          const user_id = response.userId;
          const res = await OrderService.detail(user_id);
          if (res) {
            setOrders(res);
          }
        }
      } catch (error) {
        console.error("Failed to fetch order details.");
      }
    })();
  }, []);

  return (
    <>
      <div className="page-title-area pb-0">
        <div className="container">
          <div className="page-title-content">
            <h2>Đơn hàng của bạn</h2>
          </div>
        </div>
      </div>
      <section className="content-body my-2 p-5">
        <table className="table">
          <thead>
            <tr>
              <th className="text-center">#</th>
              <th>Họ tên khách hàng</th>
              <th>Điện thoại</th>
              <th>Email</th>
              <th>Ngày đặt hàng</th>
            </tr>
          </thead>
          <tbody>
            {orders.length > 0 &&
              orders.map((order, index) => {
                return (
                  <tr className="datarow" key={index}>
                    <td></td>
                    <td>
                      <div className="name pt-3">{order.name}</div>
                    </td>
                    <td className="pt-3">{order.phone}</td>
                    <td className="pt-3">{order.email}</td>
                    <td className="pt-3">{order.createdAt}</td>
                  </tr>
                );
              })}
          </tbody>
        </table>
      </section>
    </>
  );
};

export default Order;
