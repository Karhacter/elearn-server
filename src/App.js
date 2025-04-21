import { useRoutes } from "react-router-dom";
import React from "react";
import "bootstrap/dist/js/bootstrap.min.js";
import "../src/asset/css/index.css";
import LayoutFrontend from "./layouts/frontend";
import LayoutBackend from "./layouts/backend";
import RouterSite from "./router/FrontendRouter";
import RouterAdmin from "./router/BackendRouter";
import NoPage from "../src/pages/frontend/NoPage";
import LoginForm from "./pages/frontend/account/LoginForm";
import RegisterForm from "./pages/frontend/account/RegisterForm";
import RouterLogin from "./router/RouterLogin";
import RouterRegister from "./router/RouterRegister";
import PrivateRoute from "./components/PrivateRoute";
function App() {
  let element = useRoutes([
    {
      path: "/",
      element: <LoginForm />,
      children: RouterLogin,
    },
    {
      path: "/home",
      element: <LayoutFrontend />,
      children: RouterSite,
    },
    {
      path: "register",
      element: <RegisterForm />,
      children: RouterRegister,
    },
    {
      path: "admin",
      element: (
        <PrivateRoute>
          <LayoutBackend />
        </PrivateRoute>
      ),
      children: RouterAdmin,
    },
    {
      path: "*",
      element: <NoPage />,
    },
  ]);

  return element;
}

export default App;
