using System.Collections.Generic;

namespace RoundRobinCoreApi.Models
{
    public class Round 
    {
      public Round(){}

      public long Id { get; set; }
      public List<Matchup> Matchups { get; set; }
      public Participant Bye;
  }
}