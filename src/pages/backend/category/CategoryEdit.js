import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import CategoryService from "../../../services/CategoryService";

const CategoryEdit = () => {
  const [category, setCategory] = useState({});

  const [categorys, setCategorys] = useState([]);
  const [insertId, setInsertId] = useState(0);
  //
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [slug, setSlug] = useState("");
  const [parent_id, setParent_id] = useState(0);
  const [status, setStatus] = useState(1);
  let { id } = useParams();

  useEffect(() => {
    (async () => {
      const results = await CategoryService.get_list();
      if (results.status === true) {
        setCategorys(results.categorys);
        console.log(results);
      }
      const result = await CategoryService.show(id);

      // If result.category is an array, convert it to an object
      const categoryObject = result;
      setCategory(categoryObject);
      console.log(categoryObject);
      setName(categoryObject.name);
      setDescription(categoryObject.description);
    })();
  }, []);

  const handleUpdate = (event) => {
    event.preventDefault();
    const image = document.querySelector("#image");
    const category = new FormData();
    category.append("name", name);
    category.append("description", description);

    (async () => {
      const result = await CategoryService.edit(id, category);

      alert("Cập Nhật Thành Công");
      setInsertId(result.insertId);
      window.location.href = "/admin/category";
    })();
  };

  return (
    <div className="container">
      <form onSubmit={handleUpdate}>
        <section className="content-header my-2">
          <h1 className="d-inline">Cập nhật Danh mục</h1>
          <div className="text-end">
            <Link to={"/admin/category"} className="btn btn-sm btn-info">
              <i className="fa fa-arrow-left"></i> Về danh sách
            </Link>{" "}
            <button type="submit" className="btn btn-sm btn-success">
              <i className="fa fa-save" aria-hidden="true"></i> Cập nhật
            </button>
          </div>
        </section>

        <section className="content-body my-2">
          <input name="id" value={category.id} type="hidden" />
          <div className="row">
            <div className="col-md-9">
              <div className="mb-3">
                <label>
                  <strong>Tên thương hiệu (*)</strong>
                </label>
                <input
                  type="text"
                  name="name"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  className="form-control"
                  required
                />
              </div>

              <div className="mb-3">
                <label>
                  <strong>Mô tả</strong>
                </label>
                <textarea
                  name="description"
                  rows={10}
                  className="form-control"
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                ></textarea>
              </div>
            </div>
          </div>
        </section>
      </form>
    </div>
  );
};

export default CategoryEdit;
