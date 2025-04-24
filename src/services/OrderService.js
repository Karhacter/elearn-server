import httpAxios from "../httpAxios";

const OrderService = {
  get_list: () => {
    return httpAxios.get("order");
  },
  detail: (userId) => {
    return httpAxios.get(`order/tracking/${userId}`);
  },
  update: async (id, order) => {
    return await httpAxios.put(`order/edit/${id}`, order);
  },
  orderDetail: async (id) => {
    return await httpAxios.get(`order/detail/${id}`);
  },
  store: async (order) => {
    return await httpAxios.post(`order/create`, order);
  },
  remove: async (id) => {
    return await httpAxios.delete(`order/${id}`);
  },
  
};

export default OrderService;
