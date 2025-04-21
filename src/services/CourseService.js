import httpAxios from "../httpAxios";

const CourseService = {
  get_list: () => {
    return httpAxios.get("course");
  },
  list: async (page, limit) => {
    return await httpAxios.get(`course/list/${page}/${limit}`);
  },
  detail: async (slug, limit) => {
    return await httpAxios.get(`course/coursedetail/${slug}/${limit}`);
  },
  listCourseCategory: async (categoryid) => {
    return await httpAxios.get(`course/category/${categoryid}`);
  },
  store: async (course) => {
    return await httpAxios.post("course/add", course);
  },
  remove: async (id) => {
    return await httpAxios.delete(`course/${id}`);
  },
  show: async (id) => {
    return await httpAxios.get(`course/detail/${id}`);
  },
  edit: async (id, course) => {
    return await httpAxios.put(`course/edit/${id}`, course);
  },

  uploadImage: async (courseId, formData) => {
    return await httpAxios.post(`course/${courseId}/upload-image`, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },
};

export default CourseService;
