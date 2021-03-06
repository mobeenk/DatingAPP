using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }
        [HttpGet]
        //users is COntroller class name will search first Get it finds this method
        // http://localhost:5000/api/users?pageNumber=2&pageSize=2
        // http://localhost:5000/api/users?pageNumber=5

        // http://localhost:5000/api/users?gender=male&pageNumber=2&pageSize=2
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userparams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) ;

            var userFromRepo = await _repo.GetUser(currentUserId);

            userparams.userId = currentUserId;
        // if gender not specified, we want search results
        //  to be the opposite sex of the logged in user
        //  if we specified gender in route link
            if(string.IsNullOrEmpty(userparams.Gender))
            {
                userparams.Gender = userFromRepo.Gender == "male" ? "female":"male";
            }
            // here we call the interface method by passing the gender then return the page
            var users = await _repo.GetUsers(userparams);
            //userForListDto carries specific info we want to map
             var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize,
            users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }
        // http://localhost:5000/api/users/2
        //Name used in AuthController to get the route directly
         [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);
        }
        //to update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

            throw new System.Exception($"Updating user {id} failed on save");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            // get the like from databases after passing parameters
            var like = await _repo.GetLike(id, recipientId);
            if(like != null)
            {
                return BadRequest("You already liked this user");
            }

            if( await _repo.GetUser(recipientId) == null)
                return NotFound();

            like = new Like
            {
               LikerId = id,
               LikeeId = recipientId 
            };
            //  add to database
            _repo.Add<Like>(like);

            if( await _repo.SaveAll())
                return Ok();
            
            return BadRequest("failed to like user");

        }
    }
}