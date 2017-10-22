using Microsoft.AspNetCore.Builder;

namespace SnowAPI.Infrastracture
{
    /// <summary>
    /// ASP.NET core middleware extensions
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Applies log middleware to request
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <returns>Application builder with included middleware</returns>
        public static IApplicationBuilder UseApiLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiLogMiddleware>();
        }
    }
}
