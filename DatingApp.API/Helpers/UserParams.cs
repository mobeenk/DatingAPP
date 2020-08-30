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
        
    }
}