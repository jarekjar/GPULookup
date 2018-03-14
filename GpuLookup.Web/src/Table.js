import React, { Component } from 'react';
import {Card, Table} from 'react-materialize';
import * as axios from 'axios';

class GpuTable extends Component {
  state = {
    gpus: []
  }

  componentDidMount = () => {
    axios.get("http://localhost:53472/api/getAll").then(
      resp => {
        console.log(resp.data);
        this.setState({ gpus: resp.data });
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
        <div className='row'>
        <span>Sort By: </span>
        <select className="col-sm-2" style={{'display': 'block'}} selected="Price Ascending">
            <option value="priceDown">Price Descending</option>
            <option value="priceUp">Price Ascending</option>
            <option value="source">Source</option>
            <option value="card">Card</option>
        </select>
        
        </div>
          <Card>
            <Table className="responsive table table-hover">
              <thead>
                <tr>
                    <th>#</th>
                    <th>Card</th>
                    <th>Price</th>
                    <th>Source</th>
                </tr>
              </thead>
              {
                this.state.gpus.map((item, index) => (
                <tbody key={index}>
                  <tr>
                    <td>{index + 1}</td>
                    <td>{item.Card}</td>
                    <td>${item.Price}</td>
                    <td>{item.Source}</td>
                  </tr>
                </tbody>
                ))
              }
            </Table>
          </Card>
        </div>
      </div>
    );
  }
}

export default GpuTable;

