using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsPortal.Persistence;
using NewsPortal.Persistence.DTO;

namespace NewsPortal.WebService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<Editor> _signInManager;

        public AccountController(SignInManager<Editor> signInManager)
        {
            _signInManager = signInManager;
        }

        // api/Account/
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            Editor user = await _signInManager.UserManager.GetUserAsync(User);
            if(user == null)
            {
                return Unauthorized();
            }

            EditorDTO responseData = new EditorDTO
            {
                Username = user.UserName,
                Name = user.Name,
                Address = user.Address,
                Phone = user.PhoneNumber,
                ArticleCount = user.Articles.Count
            };

            return Ok(responseData);

        }

        // api/Account/Login
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDTO user)
        {
            if (_signInManager.IsSignedIn(User))
                await _signInManager.SignOutAsync();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Username, user.Password, isPersistent: false,
                    lockoutOnFailure: false);

               
                if (result.Succeeded)
                {
                    return Ok();
                }

                ModelState.AddModelError("", "Bejelentkezés sikertelen!");
                return Unauthorized();
            }

            return Unauthorized();
        }

        [HttpPost("Signout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }
    }
}