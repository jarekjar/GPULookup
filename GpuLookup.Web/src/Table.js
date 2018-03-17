import React, { Component } from 'react';
import {Card, Table, Autocomplete, Button, Pagination} from 'react-materialize';
import * as axios from 'axios';

class GpuTable extends Component {
  state = {
    gpus: [],
    searchValue: '',
    sort: {
      PageNum: 1,
      Ascending: true,
      SortBy: "price"
    }
  }

  

  componentDidMount = () => {
    this.getGpus();
  }

  getGpus = () => {
    axios.post("http://localhost:53472/api/getNext", this.state.sort)
    .then(
      resp => {
        this.setState({ gpus: resp.data });
      },
      err => {
        console.log(err);
      }
    )
  }

  searchBar = '';

  sortItems = (e) => {
    const option = e.target.value;
    if (option === "priceHigh"){
      this.setState({
        sort: {
          PageNum: 1,
          Ascending: false,
          SortBy: "price"
        }
      }, this.getGpus)
    } else if (option === "priceLow"){
      this.setState({
        sort: {
          PageNum: 1,
          Ascending: true,
          SortBy: "price"
        }
      }, this.getGpus)
    } else if (option === "Source"){
      this.setState({
        sort: {
          PageNum: 1,
          Ascending: true,
          SortBy: "source"
        }
      }, this.getGpus)
    } else if (option === "Card"){
      this.setState({
        sort: {
          PageNum: 1,
          Ascending: true,
          SortBy: "chip"
        }
      }, this.getGpus)
    }
  }

  search = () => {
  //   const query = encodeURIComponent(this.searchBar.state.value);
  //   axios.get(`http://localhost:53472/api/search/${query}`).then(
  //     resp => {
  //       console.log(resp.data);
  //       if(resp.data)
  //         this.setState({ gpus: resp.data.sort((a,b) => a.Price - b.Price) });
  //       else
  //         this.setState({gpus: []})
  //       this.searchBar.setState({ value : ''})
  //     },
  //     err => {
  //       console.log(err);
  //     }
  //   )
   }

   newPage = (pageNum) => {
      let asc = this.state.sort.Ascending;
      let sort = this.state.sort.SortBy;
      this.setState({
        sort: {
          PageNum: pageNum,
          Ascending: asc,
          SortBy: sort
        }
      }, this.getGpus)
   }

  render() {
    return (
      <div className="row">
        <div className="col gpuTable">
        <div className='row'>
          <div className="selectBox col-sm-2">
            <span style={{'paddingTop': '10px'}}>Sort By: </span>
            <select style={{'display': 'block'}} onChange={this.sortItems}>
                <option value="priceLow">Lowest Price</option>
                <option value="priceHigh">Highest Price</option>
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
              ref={(e) => this.searchBar = e}
              />
          </div>
          <div className="search">
            <Button waves='light' className='light-blue darken-3' onClick={this.search}>Search</Button>
          </div>
        </div>
          <Card>
            {
              this.state.gpus[1] && 
              <Pagination items={Math.floor(this.state.gpus[0].RowCount / 10)} activePage={1} maxButtons={8} onSelect={(pagination) => this.newPage(pagination)}/>
            }
            <Table className="responsive table table-hover">
              <thead>
                <tr>
                    <th>Image</th>
                    <th>Card</th>
                    <th>Price</th>
                    <th>Source</th>
                </tr>
              </thead>
              {
                this.state.gpus.map((item, index) => (
                <tbody key={index}>
                  <tr>
                    <td>
                      <a href={item.Url}>
                        <img 
                        src={item.ImageUrl}
                        alt={"No Image"}
                        className="gpu-image"
                        />
                      </a>
                    </td>
                    <td>
                      <a href={item.Url}> {item.Card} 
                      </a>
                    </td>
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

