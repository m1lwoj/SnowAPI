using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Validators.Routes
{
    public class RouteMessages
    {
        public static string ID_NOTEMPTY = "Id cannot be empty";
        public static string NAME_NOTEMPTY = "Name cannot be empty";
        public static string DIFFICULTY_TOOHIGH = "Difficulty is too high";
        public static string DIFFICULTY_TOOLOW = "Difficulty is too low";
        public static string DIFFICULTY_NOTEMPTY = "Difficulty cannot be empty";
        public static string LINE_NOTEMPTY = "Line cannot be empty";
        public static string KILOMETERS_NOTEMPTY = "Kilometers cannot be empty";
        public static string POINT_NOTEMPTY = "Lat & Lon cannot be empty";
    }
}
