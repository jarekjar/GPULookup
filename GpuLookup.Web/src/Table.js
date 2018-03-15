import React, { Component } from 'react';
import {Card, Table, Autocomplete, Button} from 'react-materialize';
import * as axios from 'axios';

class GpuTable extends Component {
  state = {
    gpus: []
  }

  componentDidMount = () => {
    axios.get("http://localhost:53472/api/getAll").then(
      resp => {
        console.log(resp.data);
        this.setState({ gpus: resp.data.sort((a,b) => b.Price - a.Price) });
      },
      err => {
        console.log(err);
      }
    )
  }

  sortItems = (e) => {
    const option = e.target.value;
    let sortedGpus;
    if (option === "priceDown"){
      sortedGpus = this.state.gpus.sort((a,b) => b.Price - a.Price);
    } else if (option === "priceUp"){
      sortedGpus = this.state.gpus.sort((a,b) => a.Price - b.Price);
    } else {
      sortedGpus = this.state.gpus.sort((a,b) => {
        if(a[option] < b[option]) return -1;
        if(a[option] > b[option]) return 1;
        return 0;
      });
    }
    console.log(sortedGpus);
    this.setState({gpus : sortedGpus});
  }


  render() {
    return (
      <div className="row">
        <div className="col gpuTable">
        <div className='row'>
          <div className="selectBox col-sm-2">
            <span style={{'paddingTop': '10px'}}>Sort By: </span>
            <select style={{'display': 'block'}} onChange={this.sortItems}>
                <option value="priceDown">Highest Price</option>
                <option value="priceUp">Lowest Price</option>
                <option value="Source">Source</option>
                <option value="Card">Card</option>
            </select>
          </div>
          <div className="col-sm-4 searchBox">
            <Autocomplete
              title='Search..'
              data={
                {
                  'GTX 1080': null,
                  'GTX 1080 Ti': null,
                  'Nvidia': null,
                  'GTX 1070': null,
                  'GTX 1060': null,
                  'GTX 1050': null,
                  'Radeon': null,
                  'AMD': null
                }
              }
              />
          </div>
          <div className="search">
            <Button waves='light' className='light-blue darken-3'>Search</Button>
          </div>
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

