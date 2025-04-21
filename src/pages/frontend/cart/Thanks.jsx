import React from "react";
import { Link } from "react-router-dom";
import { FaCheckCircle } from "react-icons/fa"; // For a stylish checkmark icon

const ThankYouPage = () => {
  return (
    <div className="container text-center mt-5 mb-5">
      <div className="card shadow-lg p-5 border-0 rounded">
        <FaCheckCircle className="text-success" size={80} />
        <h1 className="mt-3 text-success">Thank You for Your Order!</h1>
        <p className="lead mt-2">
          Your order has been successfully placed. We appreciate your business
          and hope you enjoy your purchase!
        </p>
        <div className="mt-4">
          <Link to="/home" className="btn btn-primary btn-lg me-2">
            Continue Shopping
          </Link>
          <Link
            to="/order-tracking"
            className="btn btn-outline-secondary btn-lg"
          >
            Track Your Order
          </Link>
        </div>
      </div>
    </div>
  );
};

export default ThankYouPage;
