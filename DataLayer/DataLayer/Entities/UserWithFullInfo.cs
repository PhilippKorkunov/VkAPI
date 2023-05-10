using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
    public class UserWithFullInfo : Entity
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        [Column("Created Date")]
        public DateTime CreatedDate { get; set; }


        [Column("State")]
        public Enums.UserState UserStateCode { get; set; }

        [Column("User Group")]
        public Enums.UserGroup UserGroupCode { get; set; }

    }
}
