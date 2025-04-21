import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import CategoryService from "../../../services/CategoryService";

const CategoryList = () => {
  const [categorys, setCategorys] = useState([]);
  const [reloadToggle, setReloadToggle] = useState(false);

  useEffect(() => {
    (async () => {
      const res = await CategoryService.get_list();
      console.log("fetch: ", res);
      setCategorys(res);
    })();
  }, [reloadToggle]);

  //
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  const resetForm = () => {
    setName("");
    setDescription("");
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    const category = new FormData();
    category.append("name", name);
    category.append("description", description);

    (async () => {
      const result = await CategoryService.store(category);

      alert("Thêm Danh Mục THành Công");
      setReloadToggle(!reloadToggle);
      console.log(result);
    })();
    resetForm();
  };

  const handleDelete = async (id) => {
    const result = await CategoryService.remove(id);
    alert("Xóa Thành CÔng");
    setReloadToggle(!reloadToggle);
  };
  return (
    <section className="bg-light p-3">
      <div className="content-header my-2">
        <h2 className="d-inline">Danh mục</h2>
        <hr className="border-0" />
      </div>
      <div className="content-body my-2">
        <div className="row">
          <div className="col-md-4">
            <form onSubmit={handleSubmit}>
              <div className=" mb-3">
                <label>
                  <strong>Tên danh mục (*)</strong>
                </label>
                <input
                  type="text"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Nhập tên danh mục"
                  className="form-control"
                  required
                />
              </div>
              <div className="mb-3">
                <label>
                  <strong>Mô tả</strong>
                </label>
                <textarea
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  placeholder="Mô tả"
                  rows="4"
                  className="form-control"
                ></textarea>
              </div>

              <div className="mb-3 text-end">
                <button type="submit" className="btn btn-sm btn-success">
                  <i className="fa fa-save"></i> Lưu[Thêm]
                </button>
              </div>
            </form>
          </div>
          <div className="col-md-8">
            <div className="row mt-3 align-items-center">
              <div className="col-12">
                <ul className="list-inline ">
                  <button className="list-inline-item">
                    <Link href="#" className="text-black text-decoration-none">
                      Tất cả
                    </Link>
                  </button>
                  <button className="list-inline-item">
                    <Link href="#" className="text-black text-decoration-none">
                      Rác
                    </Link>
                  </button>
                </ul>
              </div>
            </div>
            <div className="row my-2 mt-0 align-items-center">
              <div className="col-md-6">
                <select name="" className="d-inline me-1">
                  <option value="">Hành động</option>
                  <option value="">Bỏ vào thùng rác</option>
                </select>
                <button className="btnapply">Áp dụng</button>
              </div>
              <div className="col-md-6 text-end">
                <input type="text" className="search d-inline" />
                <button className="d-inline btnsearch ms-1">Tìm kiếm</button>
              </div>
            </div>
            <table className="table table-bordered">
              <thead>
                <tr>
                  <th>Tên danh mục</th>
                  <th>Giới Thiệu</th>

                  <th className="text-center">Hành động</th>
                </tr>
              </thead>
              <tbody>
                {categorys.length > 0 &&
                  categorys.map((category, index) => {
                    return (
                      <tr className="datarow" key={index}>
                        <td>
                          <div className="name">
                            <Link to={"/admin/category/detail/" + category.id}>
                              {category.name}
                            </Link>
                          </div>
                        </td>
                        <td>
                          <p
                            style={{
                              whiteSpace: "pre-wrap",
                              wordBreak: "break-word",
                            }}
                          >
                            {category.description}
                          </p>
                        </td>

                        <td className="text-center">
                          <div className="function_style">
                            <Link
                              className="px-5 me-1 text-primary"
                              to={"/admin/category/edit/" + category.id}
                            >
                              <i className="btn btn btn-primary fa fa-edit"></i>
                            </Link>
                            <Link className="px-5 me-1 text-danger">
                              <i
                                className="btn btn btn-danger fa fa-trash"
                                onClick={() => handleDelete(category.id)}
                              ></i>
                            </Link>
                          </div>
                        </td>
                      </tr>
                    );
                  })}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </section>
  );
};

export default CategoryList;
