import * as axios from 'axios'

return function getAll (){
    axios.Get("http://localhost:53472/api/getAll");
}