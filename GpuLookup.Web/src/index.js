import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import MyNavbar from './Navbar';
import registerServiceWorker from './registerServiceWorker';

ReactDOM.render(<MyNavbar />, document.getElementById('root'));
registerServiceWorker();
