using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SPMA.Models.Optima;

namespace SPMA.Data
{
    /// <summary>
    /// DbContext used for reading and modifying ERP Optima Database
    /// </summary>
    public class OptimaDbContext : IdentityDbContext
    {
        #region Constructor
        public OptimaDbContext(DbContextOptions<OptimaDbContext> options)
            : base(options)
        {

        }
        #endregion

        #region DbQuerys
        public DbQuery<OptimaWare> OptimaWares { get; set; }
        public DbQuery<OptimaRW> OptimaRws { get; set; }
        public DbQuery<OptimaMag> OptimaMags { get; set; }
        public DbQuery<OptimaClient> OptimaClients { get; set; }
        #endregion
    }
}
