import info1 from "../../asset/images/info/i1.jpg";
import info2 from "../../asset/images/info/i2.jpg";
import info3 from "../../asset/images/info/i3.jpg";

const Home = () => {
  return (
    <>
      <section className="page-title-area">
        <div className="home-slider owl-carousel owl-theme">
          <div className="single-slider single-slider-bg-1">
            <div className="d-table">
              <div className="d-table-cell">
                <div className="container">
                  <div className="row align-items-center">
                    <div className="col-lg-12 text-center">
                      <div className="slider-content one">
                        <h1>Awesome App For Your Modern Lifestyle</h1>
                        <p>
                          Lorem ipsum dolor sit amet, consectetur adipiscing
                          elit, sed do eiusmod tempor incididunt ut labore et
                          dolore magna aliqua. Quis ipsum suspendisse ultrices
                          gravida incididunt ut.
                        </p>

                        <div className="slider-btn text-center">
                          <a href="#" className="box-btn">
                            Let’s Talk!
                          </a>
                          <a href="#" className="box-btn border-btn">
                            Know More
                          </a>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="single-slider single-slider-bg-2">
            <div className="d-table">
              <div className="d-table-cell">
                <div className="container">
                  <div className="row align-items-center">
                    <div className="col-lg-12 text-center">
                      <div className="slider-content one">
                        <h1>Digital Agency & Marketing</h1>
                        <p>
                          Lorem ipsum dolor sit amet, consectetur adipiscing
                          elit, sed do eiusmod tempor incididunt ut labore et
                          dolore magna aliqua. Quis ipsum suspendisse ultrices
                          gravida incididunt ut.
                        </p>

                        <div className="slider-btn text-center">
                          <a href="#" className="box-btn">
                            Let’s Talk!
                          </a>
                          <a href="#" className="box-btn border-btn">
                            Know More!
                          </a>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="single-slider single-slider-bg-3">
            <div className="d-table">
              <div className="d-table-cell">
                <div className="container">
                  <div className="row align-items-center">
                    <div className="col-lg-12 text-center">
                      <div className="slider-content one">
                        <h1>Make Real-Life Connections With IT</h1>
                        <p>
                          Lorem ipsum dolor sit amet, consectetur adipiscing
                          elit, sed do eiusmod tempor incididunt ut labore et
                          dolore magna aliqua. Quis ipsum suspendisse ultrices
                          gravida incididunt ut.
                        </p>

                        <div className="slider-btn text-center">
                          <a href="#" className="box-btn">
                            Let’s Talk!
                          </a>
                          <a href="#" className="box-btn border-btn">
                            Know More
                          </a>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <div className="info-area pt-100 pb-70">
        <div className="container">
          <div className="row">
            <div className="col-lg-4 col-sm-6">
              <div className="single-info">
                <div className="info-img">
                  <img src={info1} alt="info" />
                </div>
                <div className="content">
                  <h3>
                    <i className="flaticon-info"></i> About Us
                  </h3>
                  <div className="arrow">
                    <a href="about.html">
                      <i className="flaticon-next-1"></i>
                    </a>
                  </div>
                </div>
              </div>
            </div>

            <div className="col-lg-4 col-sm-6">
              <div className="single-info">
                <div className="info-img">
                  <img src={info2} alt="info" />
                </div>
                <div className="content">
                  <h3>
                    <i className="flaticon-support"></i> Our Vision
                  </h3>
                  <div className="arrow">
                    <a href="#">
                      {" "}
                      <i className="flaticon-next-1"></i>
                    </a>
                  </div>
                </div>
              </div>
            </div>

            <div className="col-lg-4 col-sm-6 offset-sm-3 offset-lg-0">
              <div className="single-info si-30">
                <div className="info-img">
                  <img src={info3} alt="info" />
                </div>
                <div className="content">
                  <h3>
                    <i className="flaticon-goal"></i>Our Goal
                  </h3>
                  <div className="arrow">
                    <a href="#">
                      {" "}
                      <i className="flaticon-next-1"></i>
                    </a>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <section class="home-service-area pb-70">
        <div class="container">
          <div class="section-title">
            <span>Smart Services</span>
            <h2>Paso Provide All Kind of Tech Solutions</h2>
            <p>
              {" "}
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
              eiusmod tempor incididunt ut labore et dolore magna aliqua. Quis
              ipsum suspendisse.
            </p>
          </div>

          <div class="row">
            <div class="col-lg-4 col-sm-6">
              <div class="single-service">
                <div class="service-img">
                  <img src="assets\images\service\s1.png" alt="service" />
                </div>

                <div class="service-content">
                  <h3>Visual Design</h3>
                  <p>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit.
                    Atque vel sit maxime assumenda. maiores, magnam
                  </p>

                  <a href="solutions-details.html" class="line-bnt">
                    Read More
                  </a>
                </div>
              </div>
            </div>

            <div class="col-lg-4 col-sm-6">
              <div class="single-service">
                <div class="service-img">
                  <img src="assets\images\service\s2.png" alt="service" />
                </div>

                <div class="service-content">
                  <h3>Development</h3>
                  <p>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit.
                    Atque vel sit maxime assumenda. maiores, magnam
                  </p>

                  <a href="solutions-details.html" class="line-bnt">
                    Read More
                  </a>
                </div>
              </div>
            </div>

            <div class="col-lg-4 col-sm-6">
              <div class="single-service">
                <div class="service-img">
                  <img src="assets\images\service\s3.png" alt="service" />
                </div>

                <div class="service-content">
                  <h3>QA Testing</h3>
                  <p>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit.
                    Atque vel sit maxime assumenda. maiores, magnam
                  </p>

                  <a href="solutions-details.html" class="line-bnt">
                    Read More
                  </a>
                </div>
              </div>
            </div>

            <div class="col-lg-4 col-sm-6">
              <div class="single-service">
                <div class="service-img">
                  <img src="assets\images\service\s4.png" alt="service" />
                </div>

                <div class="service-content">
                  <h3>IT Management</h3>
                  <p>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit.
                    Atque vel sit maxime assumenda. maiores, magnam
                  </p>

                  <a href="solutions-details.html" class="line-bnt">
                    Read More
                  </a>
                </div>
              </div>
            </div>

            <div class="col-lg-4 col-sm-6">
              <div class="single-service">
                <div class="service-img">
                  <img src="assets\images\service\s5.png" alt="service" />
                </div>

                <div class="service-content">
                  <h3> Cyber Security</h3>
                  <p>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit.
                    Atque vel sit maxime assumenda. maiores, magnam
                  </p>

                  <a href="solutions-details.html" class="line-bnt">
                    Read More
                  </a>
                </div>
              </div>
            </div>

            <div class="col-lg-4 col-sm-6">
              <div class="single-service">
                <div class="service-img">
                  <img src="assets\images\service\s6.png" alt="service" />
                </div>

                <div class="service-content">
                  <h3> Wireless Connectivity</h3>
                  <p>
                    Lorem ipsum dolor sit amet consectetur adipisicing elit.
                    Atque vel sit maxime assumenda. maiores, magnam
                  </p>

                  <a href="solutions-details.html" class="line-bnt">
                    Read More
                  </a>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section class="home-process-area pt-100 pb-70">
        <div class="container">
          <div class="section-title">
            <span>Working Process</span>
            <h2>We are popular because of our way of working</h2>
            <p>
              Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
              eiusmod tempor incididunt ut labore et dolore magna aliqua. Quis
              ipsum suspendisse ultrices gravida.
            </p>
          </div>
          <div class="row">
            <div class="col-lg-3 col-sm-6">
              <div class="single-process">
                <div class="icon">
                  <img src="assets\images\process\p1.png" alt="process" />
                  <span>
                    <img src="assets\images\process\next2.png" alt="shape" />
                  </span>
                </div>
                <div class="content">
                  <h3>Research Product</h3>
                  <p>
                    Lorem ipsum dolor sit amet, co nsectetur adipiscing elit,
                    sed do eiusmod tempor incididunt.
                  </p>
                </div>
              </div>
            </div>
            <div class="col-lg-3 col-sm-6">
              <div class="single-process">
                <div class="icon">
                  <img src="assets\images\process\p2.png" alt="process" />
                  <span class="pro-span">
                    <img src="assets\images\process\next2.png" alt="shape" />
                  </span>
                </div>
                <div class="content">
                  <h3>User Testing</h3>
                  <p>
                    Lorem ipsum dolor sit amet, co nsectetur adipiscing elit,
                    sed do eiusmod tempor incididunt.
                  </p>
                </div>
              </div>
            </div>
            <div class="col-lg-3 col-sm-6">
              <div class="single-process">
                <div class="icon">
                  <img src="assets\images\process\p3.png" alt="process" />
                  <span>
                    <img src="assets\images\process\next2.png" alt="shape" />
                  </span>
                </div>
                <div class="content">
                  <h3>Development</h3>
                  <p>
                    Lorem ipsum dolor sit amet, co nsectetur adipiscing elit,
                    sed do eiusmod tempor incididunt.
                  </p>
                </div>
              </div>
            </div>
            <div class="col-lg-3 col-sm-6">
              <div class="single-process">
                <div class="icon">
                  <img src="assets\images\process\p4.png" alt="process" />
                </div>
                <div class="content">
                  <h3>Product Handover</h3>
                  <p>
                    Lorem ipsum dolor sit amet, co nsectetur adipiscing elit,
                    sed do eiusmod tempor incididunt.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};

export default Home;
