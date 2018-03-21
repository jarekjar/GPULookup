import React, { Component } from 'react';
import {Card, Table, Autocomplete, Button, Pagination} from 'react-materialize';
import * as axios from 'axios';
import {Modal} from 'react-bootstrap';
import $ from 'jquery';

class GpuTable extends Component {
  state = {
    gpus: [],
    searchValue: '',
    PageNum: 1,
    Ascending: true,
    SortBy: "price",
    Query: "",
    showModal: false,
    priceChange: 0
  }

  

  componentDidMount = () => {
    this.getGpus();
  }

  getGpus = () => {
    const sortObj = {
      PageNum: this.state.PageNum,
      Ascending: this.state.Ascending,
      SortBy: this.state.SortBy,
      Query: this.state.Query
    }
    axios.post("http://localhost:53472/api/getNext", sortObj)
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
        Ascending: false,
        SortBy: "price",
        PageNum: 1
      }, this.getGpus)
    } else if (option === "priceLow"){
      this.setState({
        Ascending: true,
        SortBy: "price",
        PageNum: 1
      }, this.getGpus)
    } else if (option === "Source"){
      this.setState({
        Ascending: true,
        SortBy: "source",
        PageNum: 1
      }, this.getGpus)
    } else if (option === "Card"){
      this.setState({
        Ascending: true,
        SortBy: "chip",
        PageNum: 1
      }, this.getGpus)
    }
  }

  search = () => {
     const query = this.searchBar.state.value;
     this.setState({ 
       Query: query,
       PageNum: 1
      }, this.getGpus)
   }

   newPage = (pageNum) => {
      this.setState({
        PageNum: pageNum
      }, this.getGpus)
   }

   changePage = (e) => {
     this.changedPage = parseInt(e.target.value);
   }

   newPrice = (e) => {
     this.setState({priceChange: parseFloat(e.target.value)});
   }

   goPage = () => {
     this.newPage(this.changedPage);
   }

   update = (id) => {

    // modal pop up for editing.

    const newPrice = encodeURIComponent(this.state.priceChange);
    axios.put(`http://localhost:53472/api/update/?id=${id}&price=${newPrice}`)
    .then(
      resp => {
        this.getGpus();
      },
      err => {
        console.log(err);
      }
    )
   }

   delete = (id) => {

    // modal pop up for are you sure delete? 

    axios.delete(`http://localhost:53472/api/delete/${id}`)
    .then(
      resp => {
        this.getGpus();
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
              this.state.gpus && this.state.gpus[1] && 
              <div className="row">
                <Pagination items={Math.floor(this.state.gpus[0].RowCount / 10) || 1} activePage={this.state.PageNum || 1} maxButtons={8} onSelect={(pagination) => this.newPage(pagination)}/>
                <span className="pageCount">{Math.floor(this.state.gpus[0].RowCount / 10)} pages </span>
              </div>
            }
            <div className="inputBox pageCount">
                   <span>Go To Page: </span>
                   <input type="number" onChange={(e) => this.changePage(e)}/>
                   <a href="javascript:;" 
                      className="btn btn-sq-xs btn-primary light-blue darken-3"
                      onClick={this.goPage}
                      >
                      Go!
                   </a>
                </div>
            <Table className="responsive table table-hover">
              <thead>
                <tr>
                    <th>Image</th>
                    <th>Card</th>
                    <th>Price</th>
                    <th>Source</th>
                    <th>Edit</th>
                </tr>
              </thead>
              {
                this.state.gpus &&
                this.state.gpus.map((item, index) => (
                <tbody key={item.Id}>
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
                    <td className="editDelete">
                        <i className="fa fa-edit fa-2x text-warning"
                          data-toggle="modal" 
                          data-target="#numModal"
                          onClick={() => 
                            this.setState({priceChange: item.Price, 
                                           currentUrl: item.Url, 
                                           currentImg: item.ImageUrl, 
                                           currentCard: item.Card,
                                           currentId: item.Id
                                          })
                          }
                        >
                        </i>
                        <div id="numModal" className="modal">
                              <div className="modal-header">
                                <h5 className="modal-title">Update this price</h5>
                              </div>
                              <div className="modal-body">
                              <a href={this.state.currentUrl}>
                                <img 
                                src={this.state.currentImg}
                                alt={"No Image"}
                                style={{"margin": "0 auto"}}
                                className="gpu-image-big"
                                />
                              </a>
                              <br />
                              {
                                this.state.currentCard &&
                                <a href={this.state.currentUrl}> {this.state.currentCard.substring(0,60)}... 
                                </a>
                              }
                              <input type="number" value={this.state.priceChange} onChange={(e) => this.newPrice(e)}/>
                              <Button className='light-blue darken-3' onClick={() => this.update(this.state.currentId)} data-dismiss="modal"> Update! </Button>
                              <Button className="close-button light-blue darken-3" data-dismiss="modal"> Close </Button>
                              </div>
                        </div>
                        <i className="fa fa-ban fa-2x text-danger" 
                            data-toggle="modal" 
                            data-target="#dialogModel"
                            onClick={() => 
                              this.setState({priceChange: item.Price, 
                                             currentUrl: item.Url, 
                                             currentImg: item.ImageUrl, 
                                             currentCard: item.Card,
                                             currentId: item.Id
                                            })
                            }
                        > 
                        </i>
                        <div id="dialogModel" className="modal">
                          <div className="modal-header">
                              <h5 className="modal-title">Delete this?</h5>
                          </div>
                          <div className="modal-body">
                            <a href={this.state.currentUrl}>
                                <img 
                                src={this.state.currentImg}
                                alt={"No Image"}
                                style={{"margin": "0 auto"}}
                                className="gpu-image-big"
                                />
                            </a>
                              <br />
                              {
                                this.state.currentCard &&
                                <a href={this.state.currentUrl}> {this.state.currentCard.substring(0,60)}... 
                                </a>
                              }
                              <br />
                                <Button className='light-blue darken-3 button-spread' onClick={() => this.delete(this.state.currentId)} data-dismiss="modal"> Delete </Button>
                                <Button className="close-button light-blue darken-3 button-spread" data-dismiss="modal"> Close </Button>
                              
                          </div>
                        </div>
                    </td>
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

