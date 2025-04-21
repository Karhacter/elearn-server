import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import CartService from "../../../services/CartService";
import { urlImage } from "../../../config";

const Cart = () => {
  const [cartItems, setCartItems] = useState([]);
  const [subtotal, setSubtotal] = useState(0);
  const [loadingItem, setLoadingItem] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    fetchCartItems();
  }, []);

  const fetchCartItems = async () => {
    try {
      const data = await CartService.getCart();
      if (data?.product?.length) {
        setCartItems(data.product);
        calculateSubtotal(data.product);
      } else {
        setCartItems([]);
        setSubtotal(0);
      }
    } catch (error) {
      console.error("Error fetching cart items:", error);
    }
  };

  const calculateSubtotal = (items) => {
    const total = items.reduce((acc, item) => acc + item.price * item.qty, 0);
    setSubtotal(total);
  };

  const handleQuantityChange = async (item, delta) => {
    const newQuantity = item.qty + delta;
    if (newQuantity < 1) return;
    setLoadingItem(item.product_id);
    try {
      await CartService.updateCartItem(item.product_id, { qty: newQuantity });
      fetchCartItems();
    } catch (error) {
      console.error("Error updating quantity:", error);
    } finally {
      setLoadingItem(null);
    }
  };

  const handleRemoveItem = async (product_id) => {
    setLoadingItem(product_id);
    try {
      await CartService.delete(product_id);
      fetchCartItems();
    } catch (error) {
      console.error("Error removing item:", error);
    } finally {
      setLoadingItem(null);
    }
  };

  return (
    <div className="container my-5">
      <h1 className="text-center mb-4">Your Cart</h1>
      <div className="table-responsive">
        <table className="table table-bordered table-hover text-center">
          <thead className="table-dark">
            <tr>
              <th>Image</th>
              <th>Product</th>
              <th>Price</th>
              <th>Quantity</th>
              <th>Total</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {cartItems.length > 0 ? (
              cartItems.map((item) => (
                <tr key={item.product_id}>
                  <td>
                    <img
                      src={`${urlImage}products/${item.image}`}
                      alt={item.name}
                      className="img-thumbnail"
                      style={{ width: "70px" }}
                    />
                  </td>
                  <td>{item.name}</td>
                  <td>${item.price.toFixed(2)}</td>
                  <td>
                    <div className="input-group justify-content-center">
                      <button
                        className="btn btn-outline-secondary"
                        onClick={() => handleQuantityChange(item, -1)}
                        disabled={loadingItem === item.product_id}
                      >
                        -
                      </button>
                      <input
                        type="text"
                        value={item.qty}
                        readOnly
                        className="form-control text-center mx-1"
                        style={{ maxWidth: "50px" }}
                      />
                      <button
                        className="btn btn-outline-secondary"
                        onClick={() => handleQuantityChange(item, 1)}
                        disabled={loadingItem === item.product_id}
                      >
                        +
                      </button>
                    </div>
                  </td>
                  <td>${(item.price * item.qty).toFixed(2)}</td>
                  <td>
                    <button
                      className="btn btn-danger btn-sm"
                      onClick={() => handleRemoveItem(item.product_id)}
                      disabled={loadingItem === item.product_id}
                    >
                      Remove
                    </button>
                  </td>
                </tr>
              ))
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
        <Link to="/home/product-all" className="btn btn-outline-primary">
          Continue Shopping
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
  );
};

export default Cart;
