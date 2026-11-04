import { StrictMode } from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";

import App from "./App";
import { initializeAuth } from "./features/auth/api/initializeAuth";

import "./index.css";

async function bootstrap() {
  await initializeAuth();

  ReactDOM.createRoot(document.getElementById("root")!).render(
    <StrictMode>
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </StrictMode>,
  );
}

bootstrap();
