import axios from "axios";
import { urlApi } from "./config";

const httpAxios = axios.create({
  baseURL: urlApi,
  //timeout: 1000,
  headers: { 
    "X-custom-header": "foobar",
    "Content-Type": "application/json"
  },
});

// Add a request interceptor to include JWT token in Authorization header
httpAxios.interceptors.request.use(
  function (config) {
    const token = localStorage.getItem("sessionToken");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  function (error) {
    return Promise.reject(error);
  }
);

httpAxios.interceptors.response.use(function (response) {
  return response.data;
});

export default httpAxios;