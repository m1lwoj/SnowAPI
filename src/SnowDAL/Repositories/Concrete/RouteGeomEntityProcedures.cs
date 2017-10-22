namespace SnowDAL.Repositories.Concrete
{
    public static class RouteGeomEntityProcedures 
    {
        public static string GETALL_PROCEDURE =>  "___GetRoutesGeom()";
        public static string ISVALIDROUTE_PROCEDURE => "___checkroutevalidity";
        public static string GETINRANGE_PROCEDURE => "___getroutesinrange";
        public static string GETINRANGECOUNT_PROCEDURE => "___getroutesinrangecount";
        public static string GETROUTEPOINT_PROCEDURE => "___getroutepoint";
    }
}
