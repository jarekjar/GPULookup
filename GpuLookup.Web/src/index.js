import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import MyNavbar from './Navbar';
import GpuTable from './Table';
import registerServiceWorker from './registerServiceWorker';

ReactDOM.render(<MyNavbar />, document.getElementById('root'));
ReactDOM.render(<GpuTable />, document.getElementById('body'));
registerServiceWorker();
