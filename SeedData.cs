// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Threading.Tasks;

using RoomMonitor.Data;
using RoomMonitor.Models;

namespace RoomMonitor
{
    public static class SeedData
    {

        public static async Task SeedTestUsers(UserManager<ApplicationUser> userManager, HotelStore hotelStore)
        {
            var systemAdmin = userManager.FindByNameAsync("system_admin@test.com").Result;
            if (systemAdmin == null)
            {
                systemAdmin = new ApplicationUser
                {
                    Email = "system_admin@test.com",
                    Role = RoomMonitorConstants.UserRoles.SystemAdmin
                };
                var result = await userManager.CreateAsync(systemAdmin, "test");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("systemAdmin created");
            }
            else
            {
                Log.Debug("systemAdmin already exists");
            }


            Hotel hotel = new Hotel { HotelChain = "TestHotel1", CountryCode = "GR", Town = "Athens", Suburb = "Aigaleo", NumStar = 5 };

            if (!await hotelStore.CheckExistanceAsync(hotel.GetKey()))
            {
                await hotelStore.CreateAsync(hotel);
                Log.Debug("TestHotel1 created");
            }
            else
            {
                Log.Debug("TestHotel1 already exists");
            }


            var hotelAdmin = userManager.FindByNameAsync("hotel_admin@test.com").Result;
            if (hotelAdmin == null)
            {
                hotelAdmin = new ApplicationUser
                {
                    Email = "hotel_admin@test.com",
                    Role = RoomMonitorConstants.UserRoles.HotelAdmin,
                    HotelChain = hotel.HotelChain,
                    CountryCode = hotel.CountryCode,
                    Town = hotel.Town,
                    Suburb = hotel.Suburb
                };
                var result = await userManager.CreateAsync(hotelAdmin, "test");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("hotelAdmin created");
            }
            else
            {
                Log.Debug("hotelAdmin already exists");
            }

            var hotelEmployee = userManager.FindByNameAsync("hotel_employee@test.com").Result;
            if (hotelEmployee == null)
            {
                hotelEmployee = new ApplicationUser
                {
                    Email = "hotel_employee@test.com",
                    Role = RoomMonitorConstants.UserRoles.HotelEmployee,
                    HotelChain = hotel.HotelChain,
                    CountryCode = hotel.CountryCode,
                    Town = hotel.Town,
                    Suburb = hotel.Suburb
                };
                var result = await userManager.CreateAsync(hotelEmployee, "test");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                Log.Debug("hotelEmployee created");
            }
            else
            {
                Log.Debug("hotelEmployee already exists");
            }
        }

    }
}
