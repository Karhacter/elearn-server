import React, { useState, useEffect } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import OrderService from "../../../services/OrderService";
import UserService from "../../../services/UserService";
import CartService from "../../../services/CartService";
import { urlImage } from "../../../config";

const Checkout = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { cartItems = [], subtotal = 0 } = location.state || {};

  const [Name, setName] = useState("");
  const [Email, setEmail] = useState("");
  const [Phone, setPhone] = useState("");
  const [Address, setAddress] = useState("");
  const [note, setNote] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [userId, setUserId] = useState(null);

  useEffect(() => {
    const fetchUserId = async () => {
      const sessionToken = localStorage.getItem("sessionToken");
      if (!sessionToken) {
        alert("Bạn cần Đăng Nhập Để Thanh Toán");
        navigate("/");
        return;
      }
      try {
        const response = await UserService.checkAuth();
        if (response) {
          setUserId(response.userId);
        }
      } catch (error) {
        console.error("Failed to fetch user auth:", error);
      }
    };
    fetchUserId();
  }, []);

  const validateEmail = (email) => {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(String(email).toLowerCase());
  };

  const validatePhone = (phone) => {
    const re = /^[0-9]{10,15}$/;
    return re.test(String(phone));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    if (!Name || !Email || !Phone || !Address) {
      setError("Please fill in all the required fields.");
      setLoading(false);
      return;
    }

    if (!validateEmail(Email)) {
      setError("Please enter a valid email address.");
      setLoading(false);
      return;
    }

    if (!validatePhone(Phone)) {
      setError("Please enter a valid phone number.");
      setLoading(false);
      return;
    }

    const order = {
      Name,
      Email,
      Phone,
      Address,
      Note: note,
      Total: subtotal,
      StatusOrderId: 2,
      UserId: Number(userId),
      Items: cartItems.map((item) => ({
        courseID: item.courseID,
        Quantity: item.quantity,
        Price: item.price,
      })),
    };

    try {
      const result = await OrderService.store(order);

      if (result && result.orderId) {
        const newOrder = { ...order, id: result.orderId };

        // Update localStorage
        await CartService.clear(userId);

        // Navigate with new state (without cartItems)
        navigate("/home/thanks", {
          state: { order: newOrder },
          replace: true, // Prevent going back to checkout
        });
      } else {
        setError(result?.message || "Failed to save order. Please try again.");
      }
    } catch (error) {
      console.error("Error while saving order:", error);
      setError("Failed to save order. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const updateLocalStorage = (newOrder) => {
    const existingOrders = JSON.parse(localStorage.getItem("orders")) || [];
    existingOrders.push(newOrder);
    localStorage.setItem("orders", JSON.stringify(existingOrders));
  };

  return (
    <div>
      <div className="page-title-area pb-0 pt-6">
        <h1 className="font-bold text-danger">Thanh Toán</h1>
      </div>
      <section className="py-5 ">
        <div className="container">
          <div className="row">
            <h1 className="text-main">Delivery Information</h1>
            <div className="col-md-6">
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="name">Name</label>
                  <input
                    type="text"
                    name="Name"
                    className="form-control"
                    value={Name}
                    onChange={(e) => setName(e.target.value)}
                    required
                  />
                </div>
                <div className="mb-3">
                  <label htmlFor="phone">Phone</label>
                  <input
                    type="text"
                    name="Phone"
                    className="form-control"
                    value={Phone}
                    onChange={(e) => setPhone(e.target.value)}
                    required
                  />
                </div>
                <div className="mb-3">
                  <label htmlFor="email">Email</label>
                  <input
                    type="email"
                    name="Email"
                    className="form-control"
                    value={Email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                  />
                </div>
                <div className="mb-3">
                  <label htmlFor="address">Address</label>
                  <input
                    type="text"
                    name="Address"
                    className="form-control"
                    value={Address}
                    onChange={(e) => setAddress(e.target.value)}
                    required
                  />
                </div>
                <div className="mb-3">
                  <label htmlFor="note">Note</label>
                  <textarea
                    name="note"
                    className="form-control"
                    value={note}
                    onChange={(e) => setNote(e.target.value)}
                  />
                </div>
                {error && <div className="alert alert-danger">{error}</div>}
                <div className="text-end">
                  <button
                    type="submit"
                    className="btn btn-success px-4"
                    disabled={loading}
                  >
                    {loading ? "Processing..." : "Confirm"}
                  </button>
                </div>
              </form>
            </div>
            <div className="col-md-6">
              <h2 className="fs-5 text-main">Order Information</h2>
              <table className="table table-bordered">
                <thead>
                  <tr className="bg-light">
                    <th>Image</th>
                    <th>Course Title</th>
                    <th className="text-center">Quantity</th>
                    <th className="text-center">Price</th>
                    <th className="text-center">Total</th>
                  </tr>
                </thead>
                <tbody>
                  {cartItems.map((item) => (
                    <tr key={item.product_id}>
                      <td>
                        <img
                          className="img-fluid"
                          style={{ width: "100px" }}
                          src={`${urlImage}${item.image}`}
                          alt={item.name}
                        />
                      </td>
                      <td className="align-middle">{item.courseTitle}</td>
                      <td className="text-center align-middle">
                        {item.quantity}
                      </td>
                      <td className="text-center align-middle">
                        {item.price.toFixed(2)}
                      </td>
                      <td className="text-center align-middle">
                        {(item.price * item.quantity).toFixed(2)}
                      </td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr>
                    <td colSpan="4" className="text-end">
                      <strong>Subtotal:</strong>
                    </td>
                    <td className="text-center">
                      <strong>{subtotal.toFixed(2)}</strong>
                    </td>
                  </tr>
                </tfoot>
              </table>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default Checkout;
