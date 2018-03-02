using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RoundRobinCoreApi.Models;
using RoundRobinCoreApi.Utilities;
using System;
using Newtonsoft.Json;
namespace RoundRobinCoreApi.Controllers
{
  [Route("api/[controller]")]
  public class TournamentController: Controller
  {
    private readonly TournamentModelContext _context;
    public TournamentController(TournamentModelContext context)
    {
      _context = context; 
    }

    [HttpGet]
    public IActionResult GetAll()
    {
      return Ok(_context.TournamentModels
        .Include(t => t.Participants)
        .Include(t => t.Rounds)
        .ThenInclude(t => t.Matchups));

    }

    [HttpGet("{id}", Name = "GetTournament")]
    public IActionResult GetById(long id)
    {
      var item = _context.TournamentModels.FirstOrDefault(t => t.Id == id);
      if(item == null)
      {
        return NotFound();
      }

      return new ObjectResult(item);

    }

    [HttpPost]
    public IActionResult Create([FromBody] TournamentModel tournament)
    {
      if(tournament == null)
      {
        return BadRequest();
      }
      
      _context.TournamentModels.Add(tournament);
      _context.SaveChanges();

      var matchupGenerator = new MatchupGenerator(tournament);
      tournament.Rounds = matchupGenerator.GenerateRounds();
      // Console.WriteLine(JsonConvert.SerializeObject(tournament.Rounds));

      _context.TournamentModels.Update(tournament);
      _context.SaveChanges();
      return CreatedAtRoute("GetTournament", new { id = tournament.Id }, tournament);
    }

    [HttpPut("{id}")]
    public IActionResult Update(long id, [FromBody] TournamentModel tournament)
    {
      if(tournament == null || tournament.Id != id)
      {
        return BadRequest();
      }

      var toUpdate = _context.TournamentModels.FirstOrDefault(t => t.Id == id);
      if(toUpdate == null)
      {
        return NotFound();
      }

      toUpdate.IsDoubleRoundRobin = tournament.IsDoubleRoundRobin;
      toUpdate.Participants = tournament.Participants;
      toUpdate.TournamentName = tournament.TournamentName;

      _context.TournamentModels.Update(toUpdate);
      _context.SaveChanges();

      return new NoContentResult();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(long id)
    {
      var toDelete = _context.TournamentModels.FirstOrDefault(t => t.Id == id);
      if(toDelete == null){
        return NotFound();
      }

      _context.TournamentModels.Remove(toDelete);
      _context.SaveChanges();

      return new NoContentResult();
    }
  }
}