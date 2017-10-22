using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnowDAL.DBModels
{
    [Table("routepoint", Schema = "public")]
    public class RoutePointEntity : IEntityBase
    {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("pointstring")]
        public string Point { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [ForeignKey("infoid")]
        public virtual RouteInfoEntity RouteInfo { get; set; }
    }
}
