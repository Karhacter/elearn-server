import React, { createContext, useContext, useState, useEffect } from "react";
import CartService from "../../../services/CartService";

const CartContext = createContext();

export const useCart = () => useContext(CartContext);

export const CartProvider = ({ children }) => {
  const [cart, setCart] = useState(null);
  const [cartCount, setCartCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  
  useEffect(() => {
    fetchCart(); 
  }, []);

  const fetchCart = async () => {
    setLoading(true);
    try {
      const data = await CartService.getCart();
      if (data) {
        setCart(data);
        calculateCartCount(data.Items || []);
      } else {
        setCart(null);
        setCartCount(0);
      }
    } catch (err) {
      setError(err.message);
      console.error("Error fetching cart:", err);
    } finally {
      setLoading(false);
    }
  };

  const calculateCartCount = (items) => {
    const count = items.reduce((total, item) => total + item.Quantity, 0);
    setCartCount(count);
  };
  const addToCart = async (courseId, quantity = 1) => {
    setLoading(true);
    try {
      await CartService.addItem(courseId, quantity);
      await fetchCart(); // Refresh the entire cart
    } catch (err) {
      setError(err.message);
      throw err; // Re-throw to handle in components
    } finally {
      setLoading(false);
    }
  };

  const updateCartItem = async (courseId, quantity) => {
    if (quantity < 1) {
      await removeCartItem(courseId);
      return;
    }

    setLoading(true);
    try {
      // You'll need to implement this in CartService
      await CartService.updateCartItem(courseId, quantity);
      await fetchCart();
    } catch (err) {
      setError(err.message);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const removeCartItem = async (courseId) => {
    setLoading(true);
    try {
      // You'll need to implement this in CartService
      await CartService.removeItem(courseId);
      await fetchCart();
    } catch (err) {
      setError(err.message);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const clearCart = async () => {
    setLoading(true);
    try {
      // You'll need to implement this in CartService
      await CartService.clearCart();
      setCart(null);
      setCartCount(0);
    } catch (err) {
      setError(err.message);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return (
    <CartContext.Provider
      value={{
        cart,
        cartCount,
        loading,
        error,
        addToCart,
        updateCartItem,
        removeCartItem,
        clearCart,
        refreshCart: fetchCart
      }}
    >
      {children}
    </CartContext.Provider>
  );
};