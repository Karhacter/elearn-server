import httpAxios from "../httpAxios";

const CategoryService = {
  get_list: () => {
    return httpAxios.get("category");
  },
  store: async (category) => {
    return await httpAxios.post("category/add", category);
  },
  detail: async (slug, limit) => {
    return await httpAxios.get(`category/categorydetail/${slug}/${limit}`);
  },
  remove: async (id) => {
    return await httpAxios.delete(`category/${id}`);
  },
  show: async (id) => {
    return await httpAxios.get(`category/detail/${id}`);
  },
  edit: async (id, category) => {
    return await httpAxios.put(`category/edit/${id}`, category);
  },
};

export default CategoryService;
