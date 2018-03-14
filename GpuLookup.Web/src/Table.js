import React, { Component } from 'react';
import {Card, Table} from 'react-materialize';
import * as axios from 'axios';

class GpuTable extends Component {

  componentDidMount = () => {
    axios.get("http://localhost:53472/api/getAll").then(
      resp => {
        console.log(resp.data);
      },
      err => {
        console.log(err);
      }
    )
  }

  render() {
    return (
      <div className="row">
        <div className="col gpuTable">
          <Card>
            <Table className="responsive">
              <thead>
                <tr>
                    <th>Manufacturer</th>
                    <th>Card</th>
                    <th>Price</th>
                    <th>Source</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>Alvin</td>
                  <td>Eclair</td>
                  <td>$0.87</td>
                  <td>Newegg</td>
                </tr>
                <tr>
                  <td>Alan</td>
                  <td>Jellybean</td>
                  <td>$3.76</td>
                  <td>Newegg</td>
                </tr>
                <tr>
                  <td>Jonathan</td>
                  <td>Lollipop</td>
                  <td>$7.00</td>
                  <td>Newegg</td>
                </tr>
              </tbody>
            </Table>
          </Card>
        </div>
      </div>
    );
  }
}

export default GpuTable;

