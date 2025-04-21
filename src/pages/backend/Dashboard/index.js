import { Link } from "react-router-dom";
import { FiUsers, FiShoppingCart, FiDollarSign, FiBarChart } from "react-icons/fi";

const Dashboard = () => {
  return (
    <div className="p-6 bg-gray-100 min-h-screen">
      {/* Header */}
      <header className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Dashboard</h1>
        <p className="text-gray-600">Welcome back! Here’s an overview of your system.</p>
      </header>

      {/* Stats Overview */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-white p-4 rounded-md shadow flex items-center">
          <FiUsers className="text-blue-500 text-3xl" />
          <div className="ml-3">
            <h3 className="text-lg font-semibold">Total Users</h3>
            <p className="text-gray-600">1,245</p>
          </div>
        </div>
        <div className="bg-white p-4 rounded-md shadow flex items-center">
          <FiShoppingCart className="text-green-500 text-3xl" />
          <div className="ml-3">
            <h3 className="text-lg font-semibold">Orders</h3>
            <p className="text-gray-600">320</p>
          </div>
        </div>
        <div className="bg-white p-4 rounded-md shadow flex items-center">
          <FiDollarSign className="text-yellow-500 text-3xl" />
          <div className="ml-3">
            <h3 className="text-lg font-semibold">Revenue</h3>
            <p className="text-gray-600">$12,540</p>
          </div>
        </div>
        <div className="bg-white p-4 rounded-md shadow flex items-center">
          <FiBarChart className="text-red-500 text-3xl" />
          <div className="ml-3">
            <h3 className="text-lg font-semibold">Growth</h3>
            <p className="text-gray-600">+12%</p>
          </div>
        </div>
      </div>

      {/* Recent Activity */}
      <div className="mt-6 bg-white p-6 rounded-md shadow">
        <h2 className="text-lg font-semibold mb-4">Recent Activity</h2>
        <ul className="space-y-3">
          <li className="border-b pb-2">
            <span className="font-semibold">John Doe</span> placed an order for <span className="font-semibold">iPhone 14</span>.
          </li>
          <li className="border-b pb-2">
            <span className="font-semibold">Anna Smith</span> signed up as a new user.
          </li>
          <li>
            <span className="font-semibold">Michael Lee</span> completed a purchase of <span className="font-semibold">MacBook Pro</span>.
          </li>
        </ul>
      </div>
    </div>
  );
};

export default Dashboard;
