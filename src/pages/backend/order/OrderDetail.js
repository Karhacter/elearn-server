import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import OrderDetailService from "../../../services/OrderDetailSerivce";

const OrderDetail = () => {
  const [orderDetails, setOrderDetails] = useState([]);
  let { id } = useParams();

  useEffect(() => {
    (async () => {
      const result = await OrderDetailService.detail(id);
      if (result) {
        setOrderDetails(result);
        console.log(result);
      }
    })();
  }, [id]);

  const formatDate = (dateString) => {
    if (!dateString) return "";
    const options = { year: "numeric", month: "long", day: "numeric" };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  return (
    <div className="maincontent">
      <section className="content-header my-2">
        <h1 className="d-inline">Quản lý Chi Tiết Đơn Hàng</h1>
      </section>

      <section className="content-body my-2">
        <h2>Số Lượng Đơn: {orderDetails.length}</h2>
      </section>

      {orderDetails.length === 0 && <p>No order details found.</p>}

      {orderDetails.map((orderDetail, index) => (
        <section key={index} className="content-body my-2">
          <h3 className="text-danger">Đơn hàng số #{orderDetail.id}</h3>
          <table className="table table-bordered">
            <tbody>
              <tr>
                <th>ID</th>
                <td>{orderDetail.id}</td>
              </tr>
              <tr>
                <th>Price</th>
                <td>{orderDetail.price}</td>
              </tr>
              <tr>
                <th>Quantity</th>
                <td>{orderDetail.quantity}</td>
              </tr>
              <tr>
                <th>Amount</th>
                <td>{orderDetail.amount}</td>
              </tr>
            </tbody>
          </table>

          {orderDetail.order && (
            <>
              <h4>Thông tin đơn</h4>
              <table className="table table-bordered">
                <tbody>
                  <tr>
                    <th>Order ID</th>
                    <td>{orderDetail.order.orderID}</td>
                  </tr>
                  <tr>
                    <th>Name</th>
                    <td>{orderDetail.order.name}</td>
                  </tr>
                  <tr>
                    <th>Phone</th>
                    <td>{orderDetail.order.phone}</td>
                  </tr>
                  <tr>
                    <th>Email</th>
                    <td>{orderDetail.order.email}</td>
                  </tr>
                  <tr>
                    <th>Address</th>
                    <td>{orderDetail.order.address}</td>
                  </tr>
                  <tr>
                    <th>Created At</th>
                    <td>{formatDate(orderDetail.order.createdAt)}</td>
                  </tr>
                </tbody>
              </table>

              {orderDetail.order.orderDetails &&
                orderDetail.order.orderDetails.length > 0 && (
                  <>
                    <h5>Khóa Học Được Mua</h5>
                    <table className="table table-bordered">
                      <thead>
                        <tr>
                          <th>Course ID</th>
                          <th>Title</th>
                          <th>Description</th>
                          <th>Price</th>
                          <th>Discount</th>
                          <th>Image</th>
                          <th>Duration</th>
                        </tr>
                      </thead>
                      <tbody>
                        {orderDetail.order.orderDetails.map((detail, idx) => {
                          if (!detail) return null;
                          return (
                            <tr key={idx}>
                              <td>{detail.course?.courseId}</td>
                              <td>{detail.course?.title}</td>
                              <td>{detail.course?.description}</td>
                              <td>{detail.course?.price}</td>
                              <td>{detail.course?.discount}</td>
                              <td>
                                {detail.course?.image && (
                                  <img
                                    src={detail.course.image}
                                    alt={detail.course.title}
                                    style={{ width: "100px" }}
                                  />
                                )}
                              </td>
                              <td>{detail.course?.duration} days</td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </>
                )}
            </>
          )}

          {orderDetail.course && (
            <>
              <h4>Thông tin chi tiết của khóa học</h4>
              <table className="table table-bordered">
                <tbody>
                  <tr>
                    <th>Course ID</th>
                    <td>{orderDetail.course.courseId}</td>
                  </tr>
                  <tr>
                    <th>Title</th>
                    <td>{orderDetail.course.title}</td>
                  </tr>
                  <tr>
                    <th>Description</th>
                    <td>{orderDetail.course.description}</td>
                  </tr>
                  <tr>
                    <th>Price</th>
                    <td>{orderDetail.course.price}</td>
                  </tr>
                  <tr>
                    <th>Discount</th>
                    <td>{orderDetail.course.discount}</td>
                  </tr>
                  <tr>
                    <th>Image</th>
                    <td>
                      {orderDetail.course.image && (
                        <img
                          src={orderDetail.course.image}
                          alt={orderDetail.course.title}
                          style={{ width: "100px" }}
                        />
                      )}
                    </td>
                  </tr>
                  <tr>
                    <th>Duration</th>
                    <td>{orderDetail.course.duration} days</td>
                  </tr>
                </tbody>
              </table>
            </>
          )}
        </section>
      ))}
    </div>
  );
};

export default OrderDetail;
