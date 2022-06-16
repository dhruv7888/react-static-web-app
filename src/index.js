import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';

ReactDOM.render(<App />, document.getElementById('root'));



// while deploying change "build": "react-scripts build" to "build": "CI=false && react-scripts build" in package.json
    