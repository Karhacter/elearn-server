import httpAxios from "../httpAxios";

const CartService = {
  addToCart: async (userId, courseId, quantity = 1) => {
    return await httpAxios.post("cart/add-item", {
      UserId: userId,
      CourseID: courseId,
      Quantity: quantity,
    });
  },

  getCart: async (userId) => {
    return await httpAxios.get(`/cart/user/${userId}`);
  },

  removeCartItem: async (userId, courseId) => {
    return await httpAxios.delete(`cart/remove-item`, {
      data: { UserId: userId, CourseID: courseId },
    });
  },

  updateCartItem: async (userId, courseId, quantity) => {
    return await httpAxios.put(`cart/update-item`, {
      UserId: userId,
      CourseID: courseId,
      Quantity: quantity,
    });
  },
  clear: async (userId) => {
    return await httpAxios.delete(`cart/clear/${userId}`);
  },
};

export default CartService;
