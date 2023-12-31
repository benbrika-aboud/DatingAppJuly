﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            //verifier que le username n'existe pas
            if(await UserExists(registerDto.Username))
            {
                return BadRequest("the username already exists");
            }
            //creer le hash et le salt

            var user = _mapper.Map<AppUser>(registerDto);
            using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;


            //creer un nouvel utilisateur dans la base de donnees
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
        {
            //check if user exists
            var user = await _context.Users.Include(u=> u.Photos).SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());
            
            if (user == null)
            {
                return Unauthorized("invalid username");
            }
            //build the hash for the given password
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var HashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            
            // compare passwords and return the right answer
            for(int i =0; i< HashedPassword.Length; i++) 
            {
                if (HashedPassword[i] != user.PasswordHash[i])
                {
                    return Unauthorized("invalid password");
                }
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}
