using GpuLookup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GpuLookup.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class GPUController : ApiController
    {
        readonly GpuService gpuService;
        public GPUController (GpuService gpuService)
        {
            this.gpuService = gpuService;
        }

        [Route("api/getAll"), HttpGet]
        public HttpResponseMessage GetAll()
        {
            try
            {
                var gpus = gpuService.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, gpus);
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        
        [Route("api/search/{query}"), HttpGet]
        public HttpResponseMessage Search(string query)
        {
            try
            {
                var gpus = gpuService.GetResults(query);
                return Request.CreateResponse(HttpStatusCode.OK, gpus);
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("api/getNext/"), HttpPost]
        public HttpResponseMessage GetNext(Paginate data)
        {
            try
            {
                var gpus = gpuService.GetNext(data);
                return Request.CreateResponse(HttpStatusCode.OK, gpus);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("api/update/"), HttpPut]
        public HttpResponseMessage Update(int id, decimal price)
        {
            try
            {
                gpuService.Update(id, price);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("api/delete/{id}"), HttpDelete]
        public HttpResponseMessage Update(int id)
        {
            try
            {
                gpuService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public class Paginate
        {
            public int PageNum { get; set; }
            public bool Ascending { get; set; }
            public string SortBy { get; set; }
            public string Query { get; set; }
        }
    }
}