using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() 
        {
            var users = await _userRepository.GetMembersAsync();
            //var userToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
            
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username) 
        {
            var user = await _userRepository.GetMemberByUsernameAsync(username);
            if(user != null) 
            { 
                //var userToReturn = _mapper.Map<MemberDto>(user);
                return Ok(user); 
            }
            return BadRequest("User doesn't exist");
        }

        [HttpPut()]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberToUpdate)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null) return NotFound();

            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null) return NotFound();

            _mapper.Map(memberToUpdate, user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername();

            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                PublicId = result.PublicId,
                Url = result.SecureUrl.AbsoluteUri,
            };

            if (user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                //return _mapper.Map<PhotoDto>(photo);
                return CreatedAtAction( nameof(GetUser), 
                                        new { username = user.UserName}, 
                                        _mapper.Map<PhotoDto>(photo)    );
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<IActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("This is already a main photo");

            var currentMainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);

            if(currentMainPhoto != null) currentMainPhoto.IsMain = false;
            
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync())
            
                return NoContent();

            return BadRequest("Problem puttin photo as main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You can't delete the main photo");

            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if(result.Error != null) BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if( await _userRepository.SaveAllAsync() ) return Ok();

            return BadRequest("Probleme deleting photo");
        }
    }
}
