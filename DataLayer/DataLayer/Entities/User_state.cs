using DataLayer.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
    [Table("user_state")]
    public class UserState : Entity
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("code")]
        public Enums.UserState Code { get; set; }

        [Column("descriptipon")]
        public string Description { get; set; }

    }
}
