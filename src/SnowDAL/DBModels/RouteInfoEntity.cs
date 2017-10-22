using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnowDAL.DBModels
{
    [Table("routeinfo", Schema = "public")]
    public class RouteInfoEntity : IEntityBase
    {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("difficulty")]
        public short Difficulty { get; set; }

        [Column("userid")]
        public int UserId { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; }

        public virtual RouteGeomEntity Geometry { get; set; }

        public virtual RoutePointEntity Point { get; set; }
    }
}
