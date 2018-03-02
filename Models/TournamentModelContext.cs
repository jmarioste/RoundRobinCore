using Microsoft.EntityFrameworkCore;

namespace RoundRobinCoreApi.Models {
  public class TournamentModelContext: DbContext {
    public TournamentModelContext(DbContextOptions<TournamentModelContext> options) : base(options)
    {

    }

    public DbSet<TournamentModel> TournamentModels { get; set; }
  }
}