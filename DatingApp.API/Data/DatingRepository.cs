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
<<<<<<< HEAD
             var users =  _context.Users.Include(p => p.Photos)
             .OrderByDescending(u => u.LastActive).AsQueryable();
        // dont show the user in shown users
=======
             var users =  _context.Users.Include(p => p.Photos).AsQueryable();
             // users is what we send back
// dont show the user in shown users
>>>>>>> a615012c30ed823c0ab3b78f2f189102abb08157
            users = users.Where(u => u.Id !=  userparams.userId);
        //show opposite sex
            users = users.Where(u => u.Gender ==  userparams.Gender);

<<<<<<< HEAD

        if(userparams.MinAge != 18 || userparams.MaxAge != 99)
=======
            if(userparams.MinAge != 18 || userparams.MaxAge != 99)
>>>>>>> a615012c30ed823c0ab3b78f2f189102abb08157
            {
                // this return a date from : today - years (the age)
                var minDob = DateTime.Today.AddYears(-userparams.MaxAge-1);
                var maxDob = DateTime.Today.AddYears(-userparams.MinAge);
                // Console.WriteLine(minDob );
                //   Console.WriteLine(maxDob );
      
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }
<<<<<<< HEAD
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

=======
>>>>>>> a615012c30ed823c0ab3b78f2f189102abb08157

             return await PagedList<User>.CreateAsync(users,userparams.PageNumber,userparams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() >0;
        }
    }
}