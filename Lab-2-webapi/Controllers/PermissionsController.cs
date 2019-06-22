using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab_2_webapi.Models;
using Lab_2_webapi.Services;
using Lab_2_webapi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab_2_webapi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private IPermissionsService _permissionService;

        public PermissionsController(IPermissionsService permissionService)
        {
            _permissionService = permissionService;
        }

   
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,User_Manager")]
        [HttpGet("{id}")]
        public PermissionsGetModel Get(int id)
        {
            return _permissionService.GetPermission(id);

          
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,User_Manager")]
        [HttpPost("{id}")]
        public IActionResult Post(int id, PermissionPostModel permissionPostModel)
        {
            
            var result = _permissionService.PermissionUpsert(id,permissionPostModel);
            return Ok(result);
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,User_Manager")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id,[FromQuery] int permissionId)
        {

            var result = _permissionService.Delete(id, permissionId);
            return Ok(result);

        }
    }
}