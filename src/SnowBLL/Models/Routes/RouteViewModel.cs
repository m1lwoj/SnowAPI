using SnowBLL.Enums;
using SnowBLL.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models.Routes
{
    public class RouteViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public short Difficulty { get; set; }
    }
}
