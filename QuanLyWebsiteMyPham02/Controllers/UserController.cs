﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QLMP.Common.Rsp;
using QLMP.Common.Req;
using QLMP.DAL;
using QLMP.DAL.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using QLMP.BLL;
using Microsoft.AspNetCore.Authorization;

namespace QLMP.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly QuanLyMyPhamContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly UserSvc _userSvc;

        public UserController(QuanLyMyPhamContext context, IOptionsMonitor<JwtSettings> optionsMonitor, UserSvc userSvc)
        {
            _context = context;
            _jwtSettings = optionsMonitor.CurrentValue;
            _userSvc = userSvc;
        }

        [HttpPost("Login")]
        public IActionResult Validate(LoginReq model)
        {
            var res = _userSvc.AuthenticateUser(model);
            if (!res.Success)
            {
                res.SetMessage("Invalid username/password");
                return Ok(res);
            }

            var user = (User)res.Data;
            var token = GenerateToken(user);
            var response = new SingleRsp();
            response.Data=token;
            response.SetMessage("Authenticate success");
            return Ok(response);
        }

        [HttpPost("Register")]
        public IActionResult Register(UserReq model)
        {
            var res = _userSvc.CreateUser(model);
            if (!res.Success)
            {
                return Ok(res);
            }
            res.SetMessage("User registered successfully");
            res.Data = model;
            return Ok(res);
        }
        [HttpGet("GetAllUser")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllUsersExceptAdmin()
        {
            var res = _userSvc.GetAllUsersExceptAdmin();
            return Ok(res);
        }
        [HttpGet("SeachByusername")]
        [Authorize(Roles = "admin")]
        public IActionResult GetUserByUsername(string username)
        {
            var user = _userSvc.GetUserByUserName(username);
            return Ok(user);
        }
        [HttpPut("UpdateById")]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateUser(int id, [FromBody] UserReq userReq)
        {
            if (userReq == null)
            {
                return BadRequest("Invalid user data.");
            }

            var res = _userSvc.UpdateUser(id, userReq);
            res.Data = userReq;
            return Ok(res);
          
        }

        [HttpPut("UpdateByUsername")]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateUserByUsername(string username, [FromBody] UserReq userReq)
        {
            if (userReq == null)
            {
                return BadRequest("Invalid user data.");
            }

            var res = _userSvc.UpdateUserByUserName(username, userReq);
            res.Data = userReq;
            return Ok(res);
        }
        [HttpPut("UpdateUserRoleByUsername")]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateUserRoleByUsername(string username, [FromBody] string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return BadRequest("Invalid role.");
            }

            var res = _userSvc.UpdateUserRoleByUserName(username, role);
            return Ok(res);
        }


        [HttpDelete("DeleteUserById")]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteUserById(int id)
        {
            var res =_userSvc.DeleteUser(id);
            return Ok(res);
            
        }
        [HttpDelete("DeleteByUsername")]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteUserByUsername(string username)
        {
            try
            {
                var res = _userSvc.DeleteUserByUserName(username);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        private string GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtSettings.securitykey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    
                    new Claim("UserName", user.UserName),
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("TokenId", Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
