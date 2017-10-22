using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnowDAL.DBModels
{
    [Table("routegeom", Schema = "public")]
    public class RouteGeomEntity : IEntityBase
    {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("linestring")]
        public string Line { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [ForeignKey("infoid")]
        public virtual RouteInfoEntity RouteInfo { get; set; }
    }
}
