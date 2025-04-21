import { useEffect, useState } from "react";
import CourseService from "../../../services/CourseService";
import CategoryService from "../../../services/CategoryService";
import UserService from "../../../services/UserService";

const CourseCreate = () => {
  const [insertId, setInsertId] = useState(0);
  //category
  const [categorys, setCategorys] = useState([]);
  const [users, setUsers] = useState([]);
  useEffect(
    function () {
      (async () => {
        const result = await UserService.get_list();

        setUsers(result);
        console.log(result);
      })();
    },
    [insertId]
  );
  useEffect(
    function () {
      (async () => {
        const result = await CategoryService.get_list();

        setCategorys(result);
        console.log(result);
      })();
    },
    [insertId]
  );

  // Course
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [duration, setDuration] = useState("");
  const [price, setPrice] = useState("");
  const [discount, setDiscount] = useState("");
  const [genreId, setGenreId] = useState("");
  const [instructorId, setInstructorId] = useState("");
  const [imageFile, setImageFile] = useState(null);
  const [thumbnail, setThumbnail] = useState("");
  const [uploading, setUploading] = useState(false);
  const [message, setMessage] = useState("");

  const handleImageChange = (e) => {
    if (e.target.files && e.target.files[0]) {
      setImageFile(e.target.files[0]);
    }
  };
  // Validation error states
  const [errors, setErrors] = useState({});

  const validate = () => {
    const newErrors = {};

    if (!title.trim()) {
      newErrors.title = "Tên Khóa Học là bắt buộc.";
    }
    if (!description.trim()) {
      newErrors.description = "Chi Tiết là bắt buộc.";
    }
    if (!genreId || genreId === "0") {
      newErrors.genreId = "Danh mục là bắt buộc.";
    }
    if (!price) {
      newErrors.price = "Giá là bắt buộc.";
    } else if (isNaN(price) || Number(price) < 0) {
      newErrors.price = "Giá phải là số hợp lệ và không âm.";
    }
    if (discount) {
      if (isNaN(discount) || Number(discount) < 0) {
        newErrors.discount = "Giá Sale phải là số hợp lệ và không âm.";
      } else if (Number(discount) > Number(price)) {
        newErrors.discount = "Giá Sale không được lớn hơn Giá.";
      }
    }
    if (duration) {
      if (isNaN(duration) || Number(duration) <= 0) {
        newErrors.duration = "Thời lượng phải là số dương.";
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle image file selection and preview
  const handleSubmit = async (e) => {
    e.preventDefault();
    setMessage("");

    // Call validate and prevent submission if validation fails
    if (!validate()) {
      setMessage("Please fix the validation errors.");
      return;
    }

    try {
      // Create course without image first
      const courseData = {
        title,
        description,
        duration,
        price,
        discount,
        thumbnail,
        genreId: parseInt(genreId),
        instructorId: parseInt(instructorId),
      };

      const createResponse = await CourseService.store(courseData);
      const createdCourse = createResponse;

      if (imageFile) {
        setUploading(true);
        const formData = new FormData();
        formData.append("imageFile", imageFile);

        // Upload image for the created course
        await CourseService.uploadImage(createdCourse.courseId, formData);
        setUploading(false);
      }
      alert("Thêm Khóa Học Thành Công");
      window.location.href = "/admin/course";
    } catch (error) {
      setUploading(false);
      setMessage(
        "Error creating course: " + (error.response?.data || error.message)
      );
    }
  };

  return (
    <form onSubmit={handleSubmit} className="container">
      <div className="card">
        <div className="card-header">
          <div className="row">
            <div className="col-6">
              <strong className="text-danger">Thêm Khóa Học</strong>
            </div>
            <div className="col-6 text-end">
              <button
                className="btn btn-sm btn-success"
                type="submit"
                disabled={uploading}
              >
                {uploading ? "Uploading..." : "Create Course"}
              </button>
            </div>
          </div>
        </div>
        <div className="card-body">
          <div className="row">
            <div className="col-md-9">
              <div className="mb-3">
                <label>
                  <strong>Tên Khóa Học</strong>
                </label>
                <input
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  type="text"
                  className={`form-control ${errors.title ? "is-invalid" : ""}`}
                />
                {errors.title && (
                  <div className="invalid-feedback">{errors.title}</div>
                )}
              </div>

              <div className="mb-3">
                <label>
                  <strong>Chi Tiết</strong>
                </label>
                <textarea
                  rows={5}
                  className={`form-control ${errors.description ? "is-invalid" : ""}`}
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                ></textarea>
                {errors.description && (
                  <div className="invalid-feedback">{errors.description}</div>
                )}
              </div>
              <div className="mb-3">
                <label>
                  <strong>Thumbnail</strong>
                </label>
                <textarea
                  value={thumbnail}
                  onChange={(e) => setThumbnail(e.target.value)}
                  rows={5}
                  className="form-control"
                ></textarea>
              </div>
            </div>
            <div className="col-md-3">
              <div className="mb-3">
                <label>
                  <strong>Danh muc</strong>
                </label>
                <select
                  value={genreId}
                  onChange={(e) => setGenreId(e.target.value)}
                  className={`form-control ${errors.genreId ? "is-invalid" : ""}`}
                >
                  <option value="0">Chon danh muc</option>
                  {categorys &&
                    categorys.map((category) => {
                      return (
                        <option value={category.id} key={category.id}>
                          {category.name}
                        </option>
                      );
                    })}
                </select>
                {errors.genreId && (
                  <div className="invalid-feedback">{errors.genreId}</div>
                )}
              </div>

              <div className="mb-3">
                <label>
                  <strong>Gia</strong>
                </label>
                <input
                  value={price}
                  onChange={(e) => setPrice(e.target.value)}
                  type="number"
                  className={`form-control ${errors.price ? "is-invalid" : ""}`}
                />
                {errors.price && (
                  <div className="invalid-feedback">{errors.price}</div>
                )}
              </div>
              <div className="mb-3">
                <label>
                  <strong>Gia Sale</strong>
                </label>
                <input
                  value={discount}
                  onChange={(e) => setDiscount(e.target.value)}
                  className={`form-control ${errors.discount ? "is-invalid" : ""}`}
                  type="number"
                />
                {errors.discount && (
                  <div className="invalid-feedback">{errors.discount}</div>
                )}
              </div>

              <div className="mb-3">
                <label>
                  <strong>Thời lượng</strong>
                </label>
                <input
                  value={duration}
                  onChange={(e) => setDuration(e.target.value)}
                  type="number"
                  className={`form-control ${errors.duration ? "is-invalid" : ""}`}
                />
                {errors.duration && (
                  <div className="invalid-feedback">{errors.duration}</div>
                )}
              </div>

              <div className="mb-3">
                <label>
                  <strong>Giảng viên</strong>
                </label>
                <select
                  value={instructorId}
                  onChange={(e) => setInstructorId(e.target.value)}
                  type="number"
                  className={`form-control ${errors.instructorId ? "is-invalid" : ""}`}
                >
                  <option value="0">Chon Giảng Viên</option>
                  {users &&
                    users.map((user) => {
                      return (
                        <option value={user.userId} key={user.id}>
                          {user.fullName}
                        </option>
                      );
                    })}
                </select>
              </div>

              <div className="mb-3">
                <label>
                  <strong>Hinh</strong>
                </label>
                <input
                  className="form-control"
                  type="file"
                  id="image"
                  accept="image/*"
                  onChange={handleImageChange}
                />

                {imageFile && (
                  <img
                    src={URL.createObjectURL(imageFile)}
                    alt="Preview"
                    style={{
                      marginTop: "10px",
                      maxWidth: "100%",
                      maxHeight: "200px",
                    }}
                  />
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </form>
  );
};

export default CourseCreate;
