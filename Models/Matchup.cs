namespace RoundRobinCoreApi.Models  
{
    public class Matchup
    {
      public int Id { get; set; }

      public Participant white { get; set; }
      public Participant black { get; set; }

      public Participant winner { get; set; } 
    }
}