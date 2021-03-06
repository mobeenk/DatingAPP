using System;
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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDatingRepository _repo;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        //id is passed from route
        [HttpGet("{id}",Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRepo = await _repo.GetMessage(id);
            if(messageFromRepo == null){
                return NotFound();
            }

            return Ok(messageFromRepo);
        }
        [HttpGet]
        // http://localhost:5000/api/users/12/messages?messagecontainer=Outbox
        public async Task<IActionResult> GetMessagesForUser(int userId
        ,[FromQuery] MessageParams messageParams)
        {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            //to pass the user id in route link
            messageParams.userId = userId;
            ///calling repository method and pass it a paramater from route "Outbox" or "Inbox"            
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages = _mapper.Map <IEnumerable<MessageToReturnDto>> (messagesFromRepo);
            //add to response
              Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, 
                 messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

              return Ok(messages);
        }
        //we added thread so no confusion in route with methoid GetMessage Happens
        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                  return Unauthorized();
            var messageFromRepo = await _repo.GetMessageThread(userId, recipientId);
            // var messageThread = _mapper.Map<MessageToReturnDto>(messageFromRepo);
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
            return Ok(messageThread);
        }

        [HttpPost]
        // in this request we need to send the body
         public async Task<IActionResult> CreateMessage(int userId,
          MessageForCreationDto messageForCreationDto)
         {
             var sender = await _repo.GetUser(userId);
                if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                  return Unauthorized();
                //    [Route("api/users/{userId}/[controller]")] userId is passed from route
                    // store who sent the message so it's the id of the logged in user
                messageForCreationDto.SenderId = userId;
                //  messageForCreationDto.RecipientId is being fetched from the POST request body
                var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

                if(recipient == null)
                    return BadRequest("Could not find user");
                // this means we are casting the Dto data into message class then storing all data
                // in the var message
                var message = _mapper.Map<Message>(messageForCreationDto);

                _repo.Add(message);

           
                if (await _repo.SaveAll())
                {
                    // we do reverse map to only get the required 4 paramaters
                  //to get to call method GetMessage
             // this is the response
            //       {
                //     "senderId": 5,
                //     "recipientId": 11,
                //     "messageSent": "2020-09-08T17:54:09.3906257+03:00",
                //     "content": "get off "
            //       }
                    var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                    return CreatedAtRoute("GetMessage", new {userId, id = message.Id}, messageToReturn);
                }
                throw new Exception("Creating the message failed on save");
         }
    // post request to delete message by id from 1 side of the conversation
          [HttpPost("{id}")]
          public async Task<IActionResult> DeleteMessage(int id, int userId)
          {
              /*
              id = 12 the logged in user so this is an incoming message in inbox
                    [
                        {
                            "id":12,
                            "senderId":5,
                            "senderKnownAs":"Mcdaniel",
                            "senderPhotoUrl":"https://randomuser.me/api/portraits/men/1.jpg",
                            "recipientId":12,
                            "recipientKnownAs":"Etta",
                            "recipientPhotoUrl":"https://randomuser.me/api/portraits/women/27.jpg",
                            "content":"fuckk off psycho",
                            "isRead":false,
                            "dateRead":null,
                            "messageSent":"2020-09-05T17:43:12.9347254"
                        }
                    ]
              */
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                 return Unauthorized();  

              var messageFromRepo = await _repo.GetMessage(id);

              if (messageFromRepo.SenderId == userId)
                 messageFromRepo.SenderDeleted = true;

              if (messageFromRepo.RecipientId == userId)
                 messageFromRepo.RecipientDeleted = true;

              if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                 _repo.Delete(messageFromRepo);

              if (await _repo.SaveAll())
                 return NoContent();

              throw new Exception("Error deleting the message");
          }

          
        [HttpPost("{id}/read")]
         public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
         {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                 return Unauthorized();  

              var message = await _repo.GetMessage(id);

              if (message.RecipientId != userId)
                 return Unauthorized();

              message.IsRead = true;
             message.DateRead = DateTime.Now;

              await _repo.SaveAll();

              return NoContent();
         }



    }
}