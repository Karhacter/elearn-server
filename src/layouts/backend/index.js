import { Outlet } from "react-router-dom";
import Header from "./Header";
import Footer from "./Footer";

const LayoutBackend = () => {
  return (
    <section className="maincontent">
      <Header />
      <div className="container-fluid py-3">
        <Outlet />
      </div>
      <Footer />
    </section>
  );
};

export default LayoutBackend;
