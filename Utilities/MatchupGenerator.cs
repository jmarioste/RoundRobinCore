using System.Collections.Generic;
using RoundRobinCoreApi.Models;
using System;
using System.Linq;

namespace RoundRobinCoreApi.Utilities
{
    public class MatchupGenerator
    {
      TournamentModel _tournament;
      public MatchupGenerator(TournamentModel tournament){
        _tournament = tournament;
      }
  
      public IEnumerable<Round> GenerateRounds()
      {
        List<Round> rounds = new List<Round>();
        var length = _tournament.Participants.ToArray().Length;
        for (int roundNumber = 1; roundNumber <= length; roundNumber++)
        {
            var previousRound = roundNumber > 1 ? rounds.Last() : null;
            var round = GenerateRoundMatchups(roundNumber, previousRound);
            rounds.Add(round);
        }
        return rounds;
      }

      private List<Participant> CreateModifiedSeed(int roundNumber)
      {
        var reversed = _tournament.Participants.ToList();
        reversed.Reverse();

        while(roundNumber > 0)
        {
          var item = reversed.ElementAt(0);
          reversed.RemoveAt(0);
          reversed.Add(item);
          roundNumber --;
        }

        return reversed;
      }

      private IEnumerable<Matchup> RemoveDuplicates(IEnumerable<Matchup> matchups, int roundNumber, Round previousRound)
      {
        if(roundNumber == 1){
          var len = matchups.ToArray().Length / 2;
          return matchups.Where((matchup, index) => index < len);
        } else {
          var playedAsWhite = previousRound.Matchups.Select(x => x.white.Id).ToList();
          var playedAsBlack = previousRound.Matchups.Select(x => x.black.Id).ToList();
          
          return matchups.Where(matchup=>{
            var whiteId = matchup.white.Id;
            var blackId = matchup.black.Id;
            var isSameColor = playedAsBlack.Contains(blackId) || playedAsWhite.Contains(whiteId);
            return !isSameColor;
          });
        }
      }
      public Round GenerateRoundMatchups(int roundNumber, Round previousRound)
      {
        var originalSeed = _tournament.Participants;
        var modifiedSeed = this.CreateModifiedSeed(roundNumber);

        var bye = originalSeed.Where((participant, index) =>
        {
          var white = participant;
          var black = modifiedSeed.ElementAt(index);
          return white.Id == black.Id;
        }).First();

        var matchups = originalSeed        
        .Select((participant, index) => {
          var white = participant;
          var black = modifiedSeed.ElementAt(index);

          var matchup = new Matchup();
          matchup.white = white;
          matchup.black = black;
          return matchup;
        })
        .Where(matchup => {
          var isNotBye = matchup.white.Id != bye.Id && matchup.black.Id != bye.Id;
          return isNotBye;
        });

        matchups = RemoveDuplicates(matchups, roundNumber, previousRound);

        var round = new Round();
        round.Bye = bye;
        round.Matchups = matchups.ToList();

        return round;
      }
    }
}