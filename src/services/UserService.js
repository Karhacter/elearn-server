import httpAxios from "../httpAxios";

const UserService = {
  get_list: () => {
    return httpAxios.get("user");
  },
  store: async (user) => {
    return await httpAxios.post("user/store", user);
  },
  remove: async (id) => {
    return await httpAxios.delete(`user/${id}`);
  },
  edit: async (id, user) => {
    return await httpAxios.put(`user/edit/${id}`, user);
  },
  show: async (id) => {
    return await httpAxios.get(`user/detail/${id}`);
  },
  login: async (user) => {
    return await httpAxios.post("auth/login", user);
  },
  checkAuth: async () => {
    return await httpAxios.get("auth/check-auth");
  },
  logout: async () => {
    return await httpAxios.post("auth/logout");
  },
};

export default UserService;
