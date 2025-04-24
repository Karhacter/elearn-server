import {
  CourseList,
  CourseCreate,
  CourseEdit,
  CourseDetail,
} from "../pages/backend/course";

import {
  UserList,
  UserCreate,
  UserEdit,
  UserDetail,
} from "../pages/backend/user";

import {
  CategoryList,
  CategoryDetail,
  CategoryEdit,
} from "../pages/backend/category";

import OrderList from "../pages/backend/order/OrderList";

import Dashboard from "../pages/backend/Dashboard";
import OrderEdit from "../pages/backend/order/OrderEdit";
import OrderDetail from "../pages/backend/order/OrderDetail";

const BackendRouter = [
  { path: "/admin", element: <Dashboard /> },

  // Course
  { path: "/admin/course", element: <CourseList /> },
  { path: "/admin/course/create", element: <CourseCreate /> },
  { path: "/admin/course/edit/:id", element: <CourseEdit /> },
  { path: "/admin/course/detail/:id", element: <CourseDetail /> },

  //Category
  { path: "/admin/category", element: <CategoryList /> },
  { path: "/admin/category/edit/:id", element: <CategoryEdit /> },
  { path: "/admin/category/detail/:id", element: <CategoryDetail /> },

  // User
  { path: "/admin/user", element: <UserList /> },
  { path: "/admin/user/create", element: <UserCreate /> },
  { path: "/admin/user/detail/:id", element: <UserDetail /> },
  { path: "/admin/user/edit/:id", element: <UserEdit /> },

  // Order
  { path: "/admin/order", element: <OrderList /> },
  { path: "/admin/order/edit/:id", element: <OrderEdit /> },
  { path: "/admin/orderdetail/:id", element: <OrderDetail /> },
];

export default BackendRouter;
