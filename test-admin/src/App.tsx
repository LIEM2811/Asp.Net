import {
  Admin,
  Resource,
  ListGuesser,
  EditGuesser,
  ShowGuesser,
  CustomRoutes,
} from "react-admin";
import { Layout } from "./Layout";
import { dataProvider } from "./dataProvider";
import { authProvider } from "./authProvider";
import { Dashboard } from "./Dashboard";
import { CategoryCreate, CategoryEdit, CategoryList } from "./component/category";
import { CategoryIcon } from '@mui/icons-material/Category';
import { Inventory2Icon } from '@mui/icons-material/Inventory2';
import { ProductCreate, ProductEdit, ProductList } from "./component/product";
import { Route } from 'react-router-dom';
import ProductImageUpdate from "./component/ProductImageUpdate";
import { CartList, CartShow } from "./component/Carts";

export const App = () => (
  <Admin authProvider={authProvider} layout={Layout} dataProvider={dataProvider} dashboard={Dashboard}>
        <CustomRoutes>
            <Route path="/products/:id/update-image" element={<ProductImageUpdate />} />
      </CustomRoutes>
        <Resource name="category" list={CategoryList} create={CategoryCreate} edit={CategoryEdit} icon={CategoryIcon} />
        <Resource name="product" list={ProductList} create ={ProductCreate} edit={ProductEdit} icon={Inventory2Icon}/>
        <Resource name="carts" list={CartList}  show={CartShow}/>

  </Admin>
);