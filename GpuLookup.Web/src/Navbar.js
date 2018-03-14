import React, { Component } from 'react';
import {Navbar, NavItem} from 'react-materialize'

class MyNavbar extends Component {
  render() {
    return (
      <Navbar brand="GPU Lookup" right className="light-blue darken-3 header">
        <NavItem href='get-started.html'>Getting started</NavItem>
        <NavItem href='components.html'>Components</NavItem>
        <NavItem href='components.html'>Stuff</NavItem>
      </Navbar>
    );
  }
}

export default MyNavbar;