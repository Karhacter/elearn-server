import Contact from "../pages/frontend/Contact";
import Login from "../pages/frontend/account/LoginForm";
import Profile from "../pages/frontend/account/profile";
import RegisterForm from "../pages/frontend/account/RegisterForm";
import Cart from "../pages/frontend/cart/Cart";
import Checkout from "../pages/frontend/cart/Checkout";
import Thankyou from "../pages/frontend/cart/Thanks";
import Course from "../pages/frontend/course/Course";
import CourseCategory from "../pages/frontend/course/CourseCategory";
import CourseDetail from "../pages/frontend/course/CourseDetail";

const FrontendRouter = [
  // product
  
  // route contact
  { path: "/home/contact", element: <Contact /> },
  { path: "/home/course", element: <Course /> },
  { path: "/home/course/detail/:id", element: <CourseDetail /> },
  { path: "/home/course/category/:id", element: <CourseCategory /> },
  // route blog

  // accout
  { path: "/home/cart", element: <Cart /> },
  { path: "/home/checkout", element: <Checkout /> },
  { path: "/home/thanks", element: <Thankyou /> },
  { path: "/home/login", element: <Login /> },
  { path: "/home/register", element: <RegisterForm /> },
  { path: "/home/profile", element: <Profile /> },
];

export default FrontendRouter;
