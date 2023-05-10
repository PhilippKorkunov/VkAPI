using System.ComponentModel.DataAnnotations.Schema;
using System;
using DataLayer.Entities;

namespace VkAppAPI.ModelViews
{
    public class FullInfoUserModelView
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public string CreatedDate { get; set; }

        public string UserStateCode { get; set; }

        public string UserGroupCode { get; set; }


        public FullInfoUserModelView(UserWithFullInfo userWithFullInfo) 
        {
            Id = userWithFullInfo.Id;
            Login = userWithFullInfo.Login;
            Password = userWithFullInfo.Password;
            CreatedDate = userWithFullInfo.CreatedDate.ToString("dd.MM.yyyy");
            UserStateCode = userWithFullInfo.UserStateCode.ToString();
            UserGroupCode = userWithFullInfo.UserGroupCode.ToString();
        }

    }
}
