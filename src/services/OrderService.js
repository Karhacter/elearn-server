import httpAxios from "../httpAxios";

const OrderService = {
  get_list: () => {
    return httpAxios.get("order");
  },
  store: async (order) => {
    return await httpAxios.post(`order/create`, order);
  },
  remove: async (id) => {
    return await httpAxios.delete(`order/${id}`);
  },
};

export default OrderService;
