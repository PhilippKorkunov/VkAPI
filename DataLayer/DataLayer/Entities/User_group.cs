using DataLayer.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities
{
    [Table("user_group")]
    public class UserGroup : Entity
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("code")]
        public Enums.UserGroup Code { get; set; }

        [Column("description")]
        public string Description { get; set; }

    }
}
