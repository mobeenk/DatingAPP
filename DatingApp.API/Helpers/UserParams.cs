namespace DatingApp.API.Helpers
{
    // THis class wee used to pass the parameters in usersController
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } =1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize ; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public int userId { get; set; } 
        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;
<<<<<<< HEAD
        public int MaxAge { get; set; } = 99;
        public string OrderBy { get; set; }
=======
        public int MaxAge { get; set; } =99;
        
>>>>>>> a615012c30ed823c0ab3b78f2f189102abb08157
    }
}