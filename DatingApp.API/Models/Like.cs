namespace DatingApp.API.Models
{
    public class Like
    {
        //  we have 2 Ids so EF can't tell which is primary key,
        // so we do it mantually in DataContent.cs
         public int LikerId { get; set; }
         public int LikeeId { get; set; }
         public User Liker { get; set; }
         public User  Likee { get; set; }
    }
}