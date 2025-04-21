import React, { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import UserService from "../services/UserService";

const PrivateRoute = ({ children }) => {
  const [authChecked, setAuthChecked] = useState(false);
  const [isAuthorized, setIsAuthorized] = useState(false);

  useEffect(() => {
    const checkAuth = async () => {
      const token = localStorage.getItem("sessionToken");
      if (!token) {
        setIsAuthorized(false);
        setAuthChecked(true);
        return;
      }
      try {
        const response = await UserService.checkAuth();
        if (response && response.role == "Admin") {
          localStorage.setItem("userRole", response.role);
          setIsAuthorized(true);
        } else {
          setIsAuthorized(false);
        }
      } catch (error) {
        console.error("Error during auth check:", error);
        setIsAuthorized(false);
      }
      setAuthChecked(true);
    };
    checkAuth();
  }, []);

  if (!authChecked) {
    return null;
  }

  if (!isAuthorized) {
    return <Navigate to="/" replace />;
  }

  return children;
};

export default PrivateRoute;
