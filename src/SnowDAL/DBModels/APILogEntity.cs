using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SnowDAL.DBModels
{
    [Table("apilog", Schema = "public")]
    public class APILogEntity
    {
        [Key, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }             // The (database) ID for the API log entry.

        [Column("application")]
        public string Application { get; set; }             // The application that made the request.

        [Column("user")]
        public string User { get; set; }                    // The user that made the request.

        [Column("machine")]
        public string Machine { get; set; }                 // The machine that made the request.

        [Column("requestipaddress")]
        public string RequestIpAddress { get; set; }        // The IP address that made the request.

        [Column("requestcontenttype")]
        public string RequestContentType { get; set; }      // The request content type.

        [Column("requestcontentbody")]
        public string RequestContentBody { get; set; }      // The request content body.

        [Column("requesturi")]
        public string RequestUri { get; set; }              // The request URI.

        [Column("requestmethod")]
        public string RequestMethod { get; set; }           // The request method (GET, POST, etc).

        [Column("requestheaders")]
        public string RequestHeaders { get; set; }          // The request headers.

        [Column("requesttimestamp")]
        public DateTime? RequestTimestamp { get; set; }     // The request timestamp.

        [Column("responsecontenttype")]
        public string ResponseContentType { get; set; }     // The response content type.

        [Column("responsecontentbody")]
        public string ResponseContentBody { get; set; }     // The response content body.

        [Column("responsestatuscode")]
        public int? ResponseStatusCode { get; set; }        // The response status code.

        [Column("responseheaders")]
        public string ResponseHeaders { get; set; }         // The response headers.

        [Column("responsetimestamp")]
        public DateTime? ResponseTimestamp { get; set; }    // The response timestamp.

        [Column("responsetime")]
        public TimeSpan? ResponseTime { get; set; }    // The response timestamp.
    }
}
