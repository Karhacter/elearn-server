import { useState } from "react";

const Contact = () => {
  return (
    <>
      <div className="page-title-area">
        <div className="container">
          <div className="page-title-content">
            <h2>Contact</h2>
            <ul>
              <li>
                <a href="index.html">Home</a>
              </li>

              <li className="active">Contact</li>
            </ul>
          </div>
        </div>
        <div className="page-shape">
          <div className="shape1">
            <img src="assets\images\shape\1.png" alt="shape" />
          </div>
          <div className="shape3">
            <img src="assets\images\shape\3.png" alt="shape" />
          </div>
          <div className="shape4">
            <img src="assets\images\shape\4.png" alt="shape" />
          </div>
          <div className="shape5">
            <img src="assets\images\shape\5.png" alt="shape" />
          </div>
          <div className="shape6">
            <img src="assets\images\shape\6.png" alt="shape" />
          </div>
        </div>
      </div>
      <section className="contact-area pt-100 pb-70">
        <div className="container">
          <div className="row">
            <div className="col-lg-4 col-sm-6">
              <div className="single-contact">
                <div className="icon">
                  <i className="bx bx-time"></i>
                </div>
                <div className="content">
                  <h3>Opening Hours</h3>
                  <p>Sun - Thu : 10:00 AM - 06:00 PM</p>
                </div>
              </div>
            </div>
            <div className="col-lg-4 col-sm-6">
              <div className="single-contact">
                <div className="icon">
                  <i className="bx bx-location-plus"></i>
                </div>
                <div className="content">
                  <h3>Address</h3>
                  <p>28/A Street, New York City, USA</p>
                </div>
              </div>
            </div>
            <div className="col-lg-4 col-sm-12">
              <div className="single-contact">
                <div className="icon">
                  <i className="bx bx-phone-call"></i>
                </div>
                <div className="content">
                  <h3>Phone</h3>
                  <p>+1 (321) 984 754</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="home-contact-area home-2-contact pb-100">
        <div className="container">
          <div className="section-title">
            <span>Contact Us</span>
            <h2>Let Us Know About Your Project Idea!</h2>
            <p>
              It is a long established fact that a reader will be distracted by
              the rea dable content of a page when looking at its layout. The
              point of using Lorem Ipsum is that it has a more.
            </p>
          </div>

          <div className="row">
            <div className="col-lg-6 col-md-6">
              <div className="content">
                <form id="contactForm">
                  <div className="row">
                    <div className="col-lg-12 col-sm-12">
                      <div className="form-group">
                        <input
                          type="text"
                          name="name"
                          id="name"
                          className="form-control"
                          required=""
                          data-error="Please enter your name"
                          placeholder="Your Name"
                        />
                        <div className="help-block with-errors"></div>
                      </div>
                    </div>

                    <div className="col-lg-12 col-sm-12">
                      <div className="form-group">
                        <input
                          type="email"
                          name="email"
                          id="email"
                          className="form-control"
                          required=""
                          data-error="Please enter your email"
                          placeholder="Your Email"
                        />
                        <div className="help-block with-errors"></div>
                      </div>
                    </div>

                    <div className="col-lg-12 col-sm-12">
                      <div className="form-group">
                        <input
                          type="text"
                          name="phone_number"
                          id="phone_number"
                          required=""
                          data-error="Please enter your number"
                          className="form-control"
                          placeholder="Your Phone"
                        />
                        <div className="help-block with-errors"></div>
                      </div>
                    </div>

                    <div className="col-lg-12 col-sm-12">
                      <div className="form-group">
                        <input
                          type="text"
                          name="msg_subject"
                          id="msg_subject"
                          className="form-control"
                          required=""
                          data-error="Please enter your subject"
                          placeholder="Subject"
                        />
                        <div className="help-block with-errors"></div>
                      </div>
                    </div>

                    <div className="col-lg-12 col-md-12">
                      <div className="form-group">
                        <textarea
                          name="message"
                          className="form-control"
                          id="message"
                          cols="30"
                          rows="5"
                          required=""
                          data-error="Write your message"
                          placeholder="Your Message"
                        ></textarea>
                        <div className="help-block with-errors"></div>
                      </div>
                    </div>

                    <div className="col-lg-12 col-md-12">
                      <button
                        type="submit"
                        className="default-btn page-btn box-btn"
                      >
                        Submit
                      </button>
                      <div
                        id="msgSubmit"
                        className="h3 text-center hidden"
                      ></div>
                      <div className="clearfix"></div>
                    </div>
                  </div>
                </form>
              </div>
            </div>

            <div className="col-lg-6 col-md-6">
              <div className="contact-img contact-img-2">
                <img src="assets\images\contact-img2.jpg" alt="contact" />
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};

export default Contact;
