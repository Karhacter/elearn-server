import Logo2 from "../../asset/logo2.png";

const Footer = () => {
  return (
    <>
      <footer className="footer-area footer-area-2 pt-100">
        <div className="container">
          <div className="row">
            <div className="col-lg-4 col-md-6">
              <div className="content">
                <div className="logo">
                  <a href="index.html">
                    <img src="assets\images\logo2.png" alt="logo" />
                  </a>
                </div>
                <p>
                  Lorem ipsum dolor sit amet, mattetur adipiscing elit, sed do
                  eiusmod.
                </p>
                <div className="subscribe">
                  <h4>Join Newsletter</h4>
                  <form className="newsletter-form" data-toggle="validator">
                    <input
                      type="email"
                      id="emails"
                      className="form-control"
                      placeholder="Your Email"
                      name="EMAIL"
                      required=""
                      autocomplete="off"
                    />

                    <button className="box-btn" type="submit">
                      Subscribe
                    </button>

                    <div
                      id="validator-newsletter"
                      className="form-result"
                    ></div>
                  </form>
                </div>

                <ul className="social">
                  <li>
                    <a href="#" target="_blank">
                      <i className="bx bxl-facebook"></i>
                    </a>
                  </li>
                  <li>
                    <a href="#" target="_blank">
                      <i className="bx bxl-twitter"></i>
                    </a>
                  </li>
                  <li>
                    <a href="#" target="_blank">
                      <i className="bx bxl-instagram"></i>
                    </a>
                  </li>
                  <li>
                    <a href="#" target="_blank">
                      <i className="bx bxl-pinterest"></i>
                    </a>
                  </li>
                </ul>
              </div>
            </div>
            <div className="col-lg-3 col-md-6">
              <div className="content ml-15">
                <h3>Our Service</h3>
                <ul className="footer-list">
                  <li>
                    <a href="#">Visual Design</a>
                  </li>
                  <li>
                    <a href="#"> Development</a>
                  </li>
                  <li>
                    <a href="#">QA & Testing</a>
                  </li>
                  <li>
                    <a href="#">IT Management</a>
                  </li>
                  <li>
                    <a href="#">Cyber Security</a>
                  </li>
                  <li>
                    <a href="#">Wireless Connection</a>
                  </li>
                </ul>
              </div>
            </div>
            <div className="col-lg-2 col-md-6">
              <div className="content">
                <h3>Quick Links</h3>
                <ul className="footer-list">
                  <li>
                    <a href="faq.html">FAQ</a>
                  </li>
                  <li>
                    <a href="solutions.html">Service</a>
                  </li>
                  <li>
                    <a href="#">Career</a>
                  </li>
                  <li>
                    <a href="privecy.html">Privacy & Policy</a>
                  </li>
                  <li>
                    <a href="terms-condition.html">Terms & Conditions</a>
                  </li>
                  <li>
                    <a href="#">Data Analysis</a>
                  </li>
                </ul>
              </div>
            </div>
            <div className="col-lg-3 col-md-6">
              <div className="content contacts">
                <h3 className="ml-40">Contact</h3>
                <ul className="footer-list foot-social">
                  <li>
                    <a href="tel:+1123456789">
                      <i className="bx bx-mobile-alt"></i> +1 123 456 789
                    </a>
                  </li>
                  <li>
                    <a href="tel:+1975456789">
                      <i className="bx bx-phone"></i> +1 975 456 789
                    </a>
                  </li>
                  <li>
                    <a href="mailto:hello@paso.com">
                      <i className="bx bxs-envelope"></i> hello@paso.com
                    </a>
                  </li>
                  <li>
                    <a href="mailto:support@paso.com">
                      <i className="bx bxs-envelope"></i> support@paso.com
                    </a>
                  </li>
                  <li>
                    <i className="bx bxs-map"></i> 28/A street, New York, USA
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>

        <div className="copy-area">
          <div className="container">
            <div className="row">
              <div className="col-lg-6">
                <ul className="menu">
                  <li>
                    <a href="index.html">Home</a>
                  </li>
                  <li>
                    <a href="about.html">About</a>
                  </li>
                  <li>
                    <a href="solutions.html">Solutions</a>
                  </li>
                  <li>
                    <a href="case.html">Case Studies</a>
                  </li>
                  <li>
                    <a href="blog.html">Blog</a>
                  </li>
                  <li>
                    <a href="contact.html">Contact</a>
                  </li>
                </ul>
              </div>
              <div className="col-lg-6 text-right">
                <p>
                  Copyright @2025 Karhacter. All Rights Reserved by
                  <a href="https://hibootstrap.com/" target="_blank">
                    HiBootstrap.com
                  </a>
                </p>
              </div>
            </div>
          </div>
        </div>
      </footer>
    </>
  );
};

export default Footer;
