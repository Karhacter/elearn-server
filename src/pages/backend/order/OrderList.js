import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import OrderService from "../../../services/OrderService";

const OrderList = () => {
  const [orders, setOrders] = useState([]);
  const [insertId, setInsertId] = useState(0);

  useEffect(() => {
    (async () => {
      const result = await OrderService.get_list();
      setOrders(result.orders);
    })();
  }, []);
  const handleDelete = async (id) => {
    const result = await OrderService.remove(id);
    if (result.status === true) {
      alert(result.message);
      setInsertId(result.order.insertId);
      window.location.reload();
    }
  };
  return (
    <div className="maincontent">
      <section className="content-header my-2">
        <h1 className="d-inline">Quản lý đơn hàng</h1>
        <div className="row mt-3 align-items-center">
          <div className="col-6">
            <ul className="list-inline ">
              <li className="list-inline-item">
                <Link href="#" className="text-black text-decoration-none">
                  Tất cả (123)
                </Link>
              </li>
              <li className="list-inline-item">
                <Link href="#" className="text-black text-decoration-none">
                  Xuất bản (12)
                </Link>
              </li>
              <li className="list-inline-item">
                <Link href="#" className="text-black text-decoration-none">
                  Rác (12)
                </Link>
              </li>
            </ul>
          </div>
          <div className="col-6 text-end">
            <input type="text" className="search d-inline" />
            <button className="d-inline btnsearch">Tìm kiếm</button>
          </div>
        </div>
        <div className="row mt-1 align-items-center">
          <div className="col-md-8">
            <select name="" className="d-inline me-1">
              <option value="">Hành động</option>
              <option value="">Bỏ vào thùng rác</option>
            </select>
            <button className="btnapply">Áp dụng</button>
            <select name="" className="d-inline me-1">
              <option value="">Chọn tháng</option>
              <option value="">Tháng 9</option>
            </select>
            <select name="" className="d-inline me-1">
              <option value="">Chọn năm</option>
            </select>
            <button className="btnfilter">Lọc</button>
          </div>
          <div className="col-md-4 text-end">
            <nav aria-label="Page navigation example">
              <ul className="pagination pagination-sm justify-content-end">
                <li className="page-item disabled">
                  <Link className="page-link">&laquo;</Link>
                </li>
                <li className="page-item">
                  <Link className="page-link" href="#">
                    1
                  </Link>
                </li>
                <li className="page-item">
                  <Link className="page-link" href="#">
                    2
                  </Link>
                </li>
                <li className="page-item">
                  <Link className="page-link" href="#">
                    3
                  </Link>
                </li>
                <li className="page-item">
                  <Link className="page-link" href="#">
                    &raquo;
                  </Link>
                </li>
              </ul>
            </nav>
          </div>
        </div>
      </section>
      <section className="content-body my-2">
        <table className="table table-bordered">
          <thead>
            <tr>
              <th className="text-center">
                <input type="checkbox" id="checkboxAll" />
              </th>
              <th>Họ tên khách hàng</th>
              <th>Điện thoại</th>
              <th>Email</th>
              <th>Ngày đặt hàng</th>
              <th scope="col">Hành động</th>
            </tr>
          </thead>
          <tbody>
            {orders.length > 0 &&
              orders.map((order, index) => {
                return (
                  <tr className="datarow" key={index}>
                    <td>
                      <input type="checkbox" id="checkId" />
                    </td>
                    <td>
                      <div className="name">
                        <Link href="#">{order.deliveryname}</Link>
                      </div>
                    </td>
                    <td>{order.deliveryphone}</td>
                    <td>{order.deliveryemail}</td>
                    <td>{order.created_at}</td>
                    <td>
                      <Link
                        className="px-1 me-1 text-primary"
                        to={"/admin/order/edit/" + order.id}
                      >
                        <i className="btn btn btn-primary fa fa-edit"></i>
                      </Link>
                      <Link className="px-1 me-1 text-danger">
                        <i
                          className="btn btn btn-danger fa fa-trash"
                          onClick={() => handleDelete(order.id)}
                        ></i>
                      </Link>
                    </td>
                  </tr>
                );
              })}
          </tbody>
        </table>
      </section>
    </div>
  );
};

export default OrderList;
