import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import CartService from "../../../services/CartService";
import { urlImage } from "../../../config";
import UserService from "../../../services/UserService";

const Cart = () => {
  const [cart, setCart] = useState(null);
  const [cartItems, setCartItems] = useState([]);

  const [subtotal, setSubtotal] = useState(0);
  const [loadingItem, setLoadingItem] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    fetchCartItems();
  }, []);

  const fetchCartItems = async () => {
    try {
      const response = await UserService.checkAuth();

      if (response?.userId) {
        const userId = response.userId;

        const res = await CartService.getCart(userId);
        const cartData = res;

        if (cartData) {
          setCart(cartData); // 🛒 full cart object
          setCartItems(cartData.items);
          calculateSubtotal(cartData.items);
        } else {
          setCart(null);
          setCartItems([]);
          setSubtotal(0);
        }
      } else {
        setCart(null);
        setCartItems([]);
        setSubtotal(0);
      }
    } catch (error) {
      console.error("❌ Lỗi khi lấy giỏ hàng:", error);
      setCart(null);
      setCartItems([]);
      setSubtotal(0);
    }
  };

  const calculateSubtotal = (items) => {
    const total = items.reduce(
      (acc, item) => acc + item.price * item.quantity,
      0
    );
    setSubtotal(total);
  };

  const handleQuantityChange = async (item, delta) => {
    const newQuantity = item.quantity + delta; // Ensure you use `item.quantity`
    if (newQuantity < 1) return; // Prevent reducing quantity below 1
    setLoadingItem(item.courseID); // Assuming `courseID` is the correct unique identifier
    try {
      const response = await UserService.checkAuth(); // Get userId from checkAuth
      if (response?.userId) {
        await CartService.updateCartItem(
          response.userId,
          item.courseID,
          newQuantity
        );
        fetchCartItems();
      }
    } catch (error) {
      console.error("Error updating quantity:", error);
    } finally {
      setLoadingItem(null);
    }
  };

  const handleRemoveItem = async (courseID) => {
    setLoadingItem(courseID);
    try {
      const response = await UserService.checkAuth(); // Get userId from checkAuth
      if (response?.userId) {
        await CartService.removeCartItem(response.userId, courseID);
        fetchCartItems();
      }
    } catch (error) {
      console.error("Error removing item:", error);
    } finally {
      setLoadingItem(null);
    }
  };

  return (
    <>
      <div className="page-title-area pb-0 pt-6">
        <h1 className="font-bold text-danger">Giỏ Hàng Của Bạn</h1>
      </div>
      <div className="container my-5">
        <div className="table-responsive">
          <table className="table table-bordered table-hover text-center">
            <thead className="table-dark">
              <tr>
                <th>Image</th>
                <th>courseTitle</th>
                <th>Price</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {cartItems.length > 0 ? (
                <>
                  {cartItems.map((item) => (
                    <tr key={item.courseID}>
                      <td>
                        <img
                          src={`${urlImage}${item.image}`}
                          alt={item.courseTitle}
                          className="img-fluid"
                          style={{ width: "100px" }}
                        />
                      </td>
                      <td>{item.courseTitle}</td>
                      <td>${item.price.toFixed(2)}</td>

                      <td></td>
                      <td>
                        <button
                          className="btn btn-danger btn-sm"
                          onClick={() => handleRemoveItem(item.courseID)}
                          disabled={loadingItem === item.courseID}
                        >
                          Remove
                        </button>
                      </td>
                    </tr>
                  ))}
                  <tr>
                    <td colSpan="4" className="text-end fw-bold">
                      Tổng cộng:
                    </td>
                    <td colSpan="2" className="fw-bold">
                      ${cart?.total.toFixed(2)}
                    </td>
                  </tr>
                </>
              ) : (
                <tr>
                  <td colSpan="6" className="text-center text-muted py-3">
                    Your cart is empty.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        <div className="d-flex justify-content-between mt-4">
          <Link to="/home/course" className="btn btn-outline-primary">
            Continue Select ?
          </Link>
          <div>
            <h4>Subtotal: ${subtotal.toFixed(2)}</h4>
            <button
              onClick={() =>
                navigate("/home/checkout", { state: { cartItems, subtotal } })
              }
              className="btn btn-success"
            >
              Proceed to Checkout
            </button>
          </div>
        </div>
      </div>
    </>
  );
};

export default Cart;
