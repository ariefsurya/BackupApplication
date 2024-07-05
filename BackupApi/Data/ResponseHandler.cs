
using Microsoft.AspNetCore.Mvc;
using Model;
using System.Net;

namespace TodosApi.Data
{
    public class ResponseHandler : ControllerBase
    {
        public IActionResult ApiReponseHandler(Object data)
        {
            return Ok(new ApiResponse
            {
                Code = (int)HttpStatusCode.OK,
                Message = "OK",
                Data = data
            });
        }
    }
}
