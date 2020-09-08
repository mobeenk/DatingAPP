using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
           _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync( u =>
             u.LikerId == userId && u.LikeeId == recipientId );
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(
                p => p.IsMain
            );
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
// this brings the users in page accodring the the logged in user gender type
        public async Task<PagedList<User>> GetUsers(UserParams userparams)
        {
             var users =  _context.Users.Include(p => p.Photos)
             .OrderByDescending(u => u.LastActive).AsQueryable();
        // dont show the user in shown users
            users = users.Where(u => u.Id !=  userparams.userId);
        //show opposite sex
            users = users.Where(u => u.Gender ==  userparams.Gender);
        //this is a parameter in route passed as boolean Likers=false or likees = true etc .. 
        if(userparams.Likers)
        {
            var userLikers = await GetUserLikes(userparams.userId, userparams.Likers);
            users = users.Where( u => userLikers.Contains(u.Id));
        }

        if(userparams.Likees)
        {
            var userLikees = await GetUserLikes(userparams.userId, userparams.Likers);
            users = users.Where( u => userLikees.Contains(u.Id));
        }

        if(userparams.MinAge != 18 || userparams.MaxAge != 99)
            {
                // this return a date from : today - years (the age)
                var minDob = DateTime.Today.AddYears(-userparams.MaxAge-1);
                var maxDob = DateTime.Today.AddYears(-userparams.MinAge);
                // Console.WriteLine(minDob );
                //   Console.WriteLine(maxDob );
      
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }
            if(!string.IsNullOrEmpty(userparams.OrderBy))
            {
                switch(userparams.OrderBy)
                {
                    case "created":
                    users = users.OrderByDescending(u => u.Created);
                    break;

                    default:
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
                }
            }


             return await PagedList<User>.CreateAsync(users,userparams.PageNumber,userparams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users
            .Include(x => x.Likers)
            .Include(x => x.Likees)
            .FirstOrDefaultAsync(u => u.Id == id);
            // to get who liked us
            if(likers){
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            //if logged in user has liked 2 people
            // Where userid = likerid then get the list of numbers that contains who we liked
            else{
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() >0;
        }

        public async Task<Message> GetMessage(int id)
        {
           return  await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }
     

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .AsQueryable();

            switch(messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientId == messageParams.userId);
                    break;
                case "Outbox":
                    messages = messages.Where(u => u.SenderId == messageParams.userId);
                    break;
                default:
                    messages = messages.Where
                    (u => u.RecipientId == messageParams.userId && u.IsRead == false);
                    break;
            }
               messages = messages.OrderByDescending(d => d.MessageSent);

               return await PagedList<Message>.CreateAsync(
                   messages, messageParams.PageNumber, messageParams.PageSize);
        }
        // for user to see his received messages
        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await  _context.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(m => m.RecipientId == userId &&  m.SenderId == recipientId ||  
             m.RecipientId== recipientId && m.SenderId == userId )
             .OrderByDescending(m => m.MessageSent) 
             .ToListAsync()
            ;
            return   messages;
        }
    }
}