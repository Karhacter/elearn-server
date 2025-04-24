import httpAxios from "../httpAxios";

const OrderDetailService = {
  detail: (orderId) => {
    return httpAxios.get(`Orderdetail/order/${orderId}`);
  },
};

export default OrderDetailService;
