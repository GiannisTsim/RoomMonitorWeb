using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using RoomMonitor.Models;
using RoomMonitor.Services;
using RoomMonitor.Data;

namespace RoomMonitor.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly EmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HotelUserStore _hotelUserStore;

        public UserController(HotelUserStore hotelUserStore, EmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _hotelUserStore = hotelUserStore;
        }


        [Authorize(RoomMonitorConstants.Policies.SystemAdminAccess)]
        [HttpGet("/api/users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllHotelUsers()
        {
            IEnumerable<ApplicationUser> result = await _hotelUserStore.FindAllHotelUsersAsync();
            return Ok(result);
        }


        [Authorize(RoomMonitorConstants.Policies.SystemOrHotelAdminAccess)]
        [HttpGet("/api/hotels/users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetHotelUsersByHotel([FromQuery] HotelKey hotelKey)
        {
            IEnumerable<ApplicationUser> result = await _hotelUserStore.FindHotelUsersByHotelAsync(hotelKey);
            return Ok(result);
        }


        [Authorize(RoomMonitorConstants.Policies.SystemOrHotelAdminAccess)]
        [HttpGet("/api/hotels/users/{personId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationUser>> GetHotelUserDetails([FromQuery] HotelKey hotelKey, int personId)
        {
            ApplicationUser result = await _hotelUserStore.FindHotelUserDetailsAsync(hotelKey, personId);

            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }


        [Authorize(RoomMonitorConstants.Policies.SystemOrHotelAdminAccess)]
        [HttpPost("/api/hotels/users")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApplicationUser>> AddHotelUser([FromQuery] HotelKey hotelKey, [FromBody] UserInvitation userInvitation)
        {

            if (userInvitation.Role == RoomMonitorConstants.UserRoles.SystemAdmin)
            {
                return Forbid();
            }

            // Hotel Admins can only invite Hotel Employees
            if (this.User.IsInRole(RoomMonitorConstants.UserRoles.HotelAdmin) && userInvitation.Role != RoomMonitorConstants.UserRoles.HotelEmployee)
            {
                return Forbid();
            }

            var user = new ApplicationUser
            {
                Email = userInvitation.Email,
                Role = userInvitation.Role,
                HotelChain = hotelKey.HotelChain,
                CountryCode = hotelKey.CountryCode,
                Town = hotelKey.Town,
                Suburb = hotelKey.Suburb
            };
            IdentityResult result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Using protocol param will force creation of an absolut url. We
                // don't want to send a relative URL by e-mail.
                var callbackUrl = Url.Action("Activate",
                                             "Account",
                                             new { userId = user.PersonId, code },
                                             protocol: Request.Scheme);

                await _emailSender.SendInvitationMail(user.Email, callbackUrl);
                return CreatedAtAction(
                    nameof(GetHotelUserDetails),
                    new { personId = user.PersonId, hotelKey.HotelChain, hotelKey.CountryCode, hotelKey.Town, hotelKey.Suburb },
                    user);
            }
            else
            {
                ModelState.AddModelError(nameof(UserInvitation.Email), "Email already taken.");
                return ValidationProblem(
                    title: "Duplicate Email",
                    detail: String.Format("Email '{0}' is already taken.", userInvitation.Email)
                    );
            }
        }


        [Authorize(RoomMonitorConstants.Policies.SystemOrHotelAdminAccess)]
        [HttpDelete("/api/hotels/users/{personId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteHotelUser([FromQuery] HotelKey hotelKey, [FromRoute] int personId)
        {
            ApplicationUser userToDelete = await _userManager.FindByIdAsync(personId.ToString());

            if (userToDelete == null)
            {
                return NotFound();
            }

            // Hotel Admins can only delete Hotel Employees
            if (User.IsInRole(RoomMonitorConstants.UserRoles.HotelAdmin)
                && userToDelete.Role != RoomMonitorConstants.UserRoles.HotelEmployee)
            {
                return Forbid();
            }

            await _hotelUserStore.DeleteHotelUserAsync(hotelKey, personId);

            return NoContent();
        }

    }
}