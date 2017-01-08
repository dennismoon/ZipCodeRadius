using Microsoft.EntityFrameworkCore;

namespace ZipCodeRadius.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiContext : DbContext
    {
        public DbSet<ZipCodeInfo> ZipCodes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
            
        }
    }
}
