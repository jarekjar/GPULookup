import React, { Component } from 'react';
import {Navbar, NavItem} from 'react-materialize'

class MyNavbar extends Component {
  render() {
    return (
      <Navbar brand="GPU Lookup" right className="light-blue darken-3 header">
        <NavItem href='get-started.html'></NavItem>
        <NavItem href='components.html'></NavItem>
        <NavItem href='components.html'></NavItem>
      </Navbar>
    );
  }
}

export default MyNavbar;