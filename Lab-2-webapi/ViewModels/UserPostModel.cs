

using Lab_2_webapi.Models;

namespace Lab_2_webapi.ViewModels
{
    public class UserPostModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
      
        public string Password { get; set; }
       

        public static User ToUser(UserPostModel userModel)
        {
           
            return new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Username = userModel.UserName,
                Email = userModel.Email,
                Password=userModel.Password,
                CreatedAt = System.DateTime.Today
        };
        }
    }
}