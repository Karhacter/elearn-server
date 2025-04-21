import { useEffect, useState } from "react";
import CategoryService from "../../../services/CategoryService";
import UserService from "../../../services/UserService";
import CourseService from "../../../services/CourseService";
import { Link, useParams } from "react-router-dom";

const CourseEdit = () => {
  let { id } = useParams();
  const [course, setCourse] = useState({});
  const [insertId, setInsertId] = useState(0);

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
  useEffect(function () {
    (async () => {
      const result = await CategoryService.get_list();
      if (result) {
        setCategorys(result);
        console.log(result);
      }
    })();
  }, []);

  //product
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
  useEffect(() => {
    (async () => {
      const result = await CourseService.show(id);

      // Assuming result.product contains the course object
      const productObject = result.course || result;
      setCourse(productObject);
      console.log(productObject);
      setTitle(productObject.title || "");
      setDescription(productObject.description || "");
      setDuration(productObject.duration || "");
      setPrice(productObject.price || "");
      setDiscount(productObject.discount || "");
      setGenreId(productObject.genreId || "");
      setInstructorId(productObject.instructorId || "");
      setImageFile(null); // Reset imageFile as file object cannot be set directly
      setThumbnail(productObject.thumbnail || "");
    })();
  }, []);

  const handleUpdate = async (e) => {
    e.preventDefault();
    setMessage("");
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

      const createResponse = await CourseService.edit(id, courseData);
      const createdCourse = createResponse;

      if (imageFile) {
        setUploading(true);
        const formData = new FormData();
        formData.append("imageFile", imageFile);

        // Upload image for the created course
        await CourseService.uploadImage(createdCourse.courseId, formData);
        setUploading(false);
      }
      alert("Cập Nhật Khóa Học Thành Công");
      window.location.href = "/admin/course";
    } catch (error) {
      setUploading(false);
      setMessage(
        "Error creating course: " + (error.response?.data || error.message)
      );
    }
  };
  return (
    <form onSubmit={handleUpdate} className="container">
      <div className="card">
        <div className="card-header">
          <div className="row">
            <div className="col-6">
              <strong className="text-danger">Cập nhật Khóa Học</strong>
            </div>
            <div className="col-6 text-end">
              <button className="btn btn-sm btn-info me-2">
                <Link to="/admin/course" className="text-decoration-none">
                  Trở Về
                </Link>
              </button>
              <button className="btn btn-sm btn-success" type="submit">
                Cập nhật
              </button>
            </div>
          </div>
        </div>
        <div className="card-body">
          <input name="id" value={id} type="hidden" />
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
                  className="form-control"
                />
              </div>

              <div className="mb-3">
                <label>
                  <strong>Chi Tiết</strong>
                </label>
                <textarea
                  rows={5}
                  className="form-control"
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                ></textarea>
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
                  className="form-control"
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
              </div>

              <div className="mb-3">
                <label>
                  <strong>Gia</strong>
                </label>
                <input
                  value={price}
                  onChange={(e) => setPrice(e.target.value)}
                  type="number"
                  className="form-control"
                />
              </div>
              <div className="mb-3">
                <label>
                  <strong>Gia Sale</strong>
                </label>
                <input
                  value={discount}
                  onChange={(e) => setDiscount(e.target.value)}
                  className="form-control"
                  type="number"
                />
              </div>

              <div className="mb-3">
                <label>
                  <strong>Thời lượng</strong>
                </label>
                <input
                  value={duration}
                  onChange={(e) => setDuration(e.target.value)}
                  type="number"
                  className="form-control"
                />
              </div>

              <div className="mb-3">
                <label>
                  <strong>Giảng viên</strong>
                </label>
                <select
                  value={instructorId}
                  onChange={(e) => setInstructorId(e.target.value)}
                  type="number"
                  className="form-control"
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

export default CourseEdit;
