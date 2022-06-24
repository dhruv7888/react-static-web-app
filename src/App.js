import React from 'react';
import Nav from './Nav.js';
import InputForms from './components/InputForms';
import ReactFlowRenderer from './ReactFlowRenderer';
import {
  BrowserRouter as Router,Routes,Route,
} from "react-router-dom";

function App() {
  return (
    <>
    <Router>
      <Nav/>
      <Routes>
        <Route index element={<InputForms />} />
        <Route path="/ReactFlowRenderer" element={< ReactFlowRenderer />} />
      </Routes>
    </Router>
    </>
  );
}


export default App;
