using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnowDAL.DBModels
{
    [Table("systemcode", Schema = "public")]
    public class SystemCodeEntity : IEntityBase
    {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("code")]
        public string Code { get; set; }

        [Column("userid")]
        public int UserId { get; set; }

        [Column("type")]
        public int Type { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("generatedate")]
        public DateTime GenerateDate { get; set; }

        [Column("expirationdate")]
        public DateTime ExpirationDate { get; set; }

        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; }
    }
}
