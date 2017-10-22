using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnowDAL.DBModels
{
    [Table("user", Schema = "public")]
    public class UserEntity : IEntityBase
    {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Column("password")]
        public string HashedPassword { get; set; }

        [Column("createdate")]
        public DateTime CreateDate { get; set; }

        [Column("lastlogin")]
        public DateTime LastLogin { get; set; }

        [Column("role")]
        public int Role { get; set; }

        [Column("status")]
        public int Status { get; set; }

        //TODO dodac sprawdzanie czy jest confirmed w niektórych akcjach
        [Column("isconfirmed")]
        public bool IsConfirmed { get; set; }


        public virtual ICollection<RouteInfoEntity> Routes { get; set; }
        public virtual ICollection<SystemCodeEntity> SystemCodes { get; set; }
    }
}
