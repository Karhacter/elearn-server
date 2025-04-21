import { Outlet } from "react-router-dom";
import { FaArrowUp } from "react-icons/fa"; // Using React Icons
import Header from "./Header";
import Footer from "./Footer";
import { useCallback, useEffect, useState } from "react";

const LayoutFrontend = () => {
  // back to top
  const [ShowGoTop, setShowGoTop] = useState(false);

  const handleScroll = useCallback(() => {
    setShowGoTop(window.scrollY > 500);
  }, []);
  useEffect(() => {
    // Attach event listener with passive mode for better performance
    window.addEventListener("scroll", handleScroll, { passive: true });

    return () => {
      // Cleanup event listener to avoid memory leaks
      window.removeEventListener("scroll", handleScroll);
    };
  }, [handleScroll]); // Add dependency to ensure correct behavior

  const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  };
  return (
    <>
      <Header />
      <Outlet />
      {ShowGoTop && (
        <button
          onClick={scrollToTop}
          className="btn btn-primary shadow-lg"
          style={{
            position: "fixed",
            right: "20px",
            bottom: "20px",
            width: "50px",
            height: "50px",
            borderRadius: "50%",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            fontSize: "20px",
            zIndex: 1000,
          }}
        >
          <FaArrowUp />
        </button>
      )}
      <Footer />
    </>
  );
};

export default LayoutFrontend;
