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
    public class UserRoleController : ControllerBase
    {
        private IPermissionsService _permissionService;

        public UserRoleController(IPermissionsService permissionService)
        {
            _permissionService = permissionService;
        }

   
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,User_Manager")]
        [HttpGet]
        public IActionResult Get()
        {
            var result = _permissionService.GetAllUserRole();
            return Ok(result);

        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,User_Manager")]
        [HttpPost]
        public IActionResult Post(UserRolePostModel UserPostModel)
        {
            
            var result = _permissionService.Upsert(UserPostModel);
            return Ok(result);
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin,User_Manager")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            var result = _permissionService.DeleteUserRole(id);
            return Ok(result);

        }
    }
}