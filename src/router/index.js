import BackendRouter from "./BackendRouter";
import FrontendRouter from "./FrontendRouter";
import RouterLogin from './RouterLogin'
import RouterRegister from './RouterRegister'

const AppRouter = {
  "RouterSite": FrontendRouter,
  "RouterAdmin": BackendRouter,
  "RouterLogin":RouterLogin,
  "RouterRegister":RouterRegister,
};

export default AppRouter;
