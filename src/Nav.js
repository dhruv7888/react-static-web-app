import React from 'react';
import styled from "styled-components";
import { NavLink } from "react-router-dom";  

const NavUnlisted = styled.ul`

  display: flex;

  a {
    text-decoration: none;
  }

  li {
    color: red;
    margin: 0 0.8rem;
    font-size: 1.3rem;
    position: relative;
    list-style: none;
  }

  .current {
    li {
      border-bottom: 2px solid black;
    }
  }
`;


function Nav() {
  return (
    <div class="bg-dark navbar-dark navbar">
    <NavUnlisted>
      <NavLink to="/" activeClassName="current" exact>
        <li>Service-Details</li>
      </NavLink>
      <NavLink to="/ReactFlowRenderer" activeClassName="current" exact>
        <li>Design-Flow</li>
      </NavLink>
    </NavUnlisted>
    </div>
  );
}
export default Nav;