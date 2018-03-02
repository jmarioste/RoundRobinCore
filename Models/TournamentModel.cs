using System.Collections.Generic;

namespace RoundRobinCoreApi.Models {
  public class TournamentModel {
    public long Id { get; set; }

    public string TournamentName { get; set; }
    public List<Participant> Participants { get; set; }
    public bool IsDoubleRoundRobin { get; set; }

    public IEnumerable<Round> Rounds { get; set; }

  }
}