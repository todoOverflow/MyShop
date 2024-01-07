import { useEffect, useState } from "react";
import Header from "./Header";
import {
  Container,
  CssBaseline,
  ThemeProvider,
  createTheme,
} from "@mui/material";
import { Outlet } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { getCookie } from "../util/util";
import LoadingComponent from "./LoadingComponent";
import { useAppDispatch, useAppSelector } from "../store/configureStore";
import { loadBasketItemsAsync } from "../../features/basket/basketSlice";

function App() {
  const dispatch = useAppDispatch();
  const { status } = useAppSelector((state) => state.basket);

  useEffect(() => {
    const buyerId = getCookie("buyerId");
    if (buyerId) {
      dispatch(loadBasketItemsAsync());
    }
  }, [dispatch]);

  const [darkMode, setDarkmode] = useState(false);
  const paletteType = darkMode ? "dark" : "light";
  const theme = createTheme({
    palette: {
      mode: paletteType,
      background: {
        default: paletteType === "light" ? "#eaeaea" : "#121212",
      },
    },
  });

  function handleThemeChange() {
    setDarkmode(!darkMode);
  }
  if (status === "pendingLoadBasketItems")
    return <LoadingComponent message="Loading Basket..." />;
  return (
    <ThemeProvider theme={theme}>
      <ToastContainer position="bottom-right" hideProgressBar theme="colored" />
      <CssBaseline></CssBaseline>
      <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
      <Container>
        <Outlet />
      </Container>
    </ThemeProvider>
  );
}

export default App;
