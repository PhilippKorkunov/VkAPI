using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
    [Table("user")]
    public class User : Entity
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("login")]
        public string Login { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("user_group_id")]
        public int UserGroupId { get; set; }

        [ForeignKey("UserGroupId")]
        public UserGroup UserGroup { get; set; }

        [Column("user_state_id")]
        public int UserStateId { get; set; }

        [ForeignKey("UserStateId")]
        public UserState UserState { get; set; }

    }
}
