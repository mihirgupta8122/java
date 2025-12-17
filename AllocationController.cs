using Microsoft.AspNetCore.Mvc;
using ParkingAllocationApi.Domain;
using ParkingAllocationApi.Interfaces;

namespace ParkingAllocationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AllocationController : ControllerBase
{
    private readonly IAllocationService _service;
    private readonly IParkingLotRepository _repo;

    public AllocationController(IAllocationService service, IParkingLotRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    [HttpPost("seed")]
    public IActionResult Seed()
    {
        if (_repo.GetAllSpots().Count == 0)
        {
            _repo.AddSpot(new ParkingSpot("P1", VehicleSize.Compact, true));
            _repo.AddSpot(new ParkingSpot("P2", VehicleSize.Large, true));
        }
        return Ok(new { message = "Seeded", spots = _repo.GetAllSpots().Select(s => new { s.Id, s.Size, s.Available }) });
    }

    [HttpGet("summary")]
    public IActionResult Summary() => Ok(_service.GetSummary());
    
    [HttpGet("sizes")]
    public IActionResult Sizes()
    {
        var allowed = Enum.GetNames(typeof(VehicleSize)); // ["Compact","Large"]
        return Ok(new { allowed });
    }

    [HttpPost("allocate")]
    public IActionResult Allocate([FromBody] Vehicle vehicle)
    {
        var spot = _service.Allocate(vehicle); // Exceptions handled by middleware
        return Ok(new { allocatedSpot = spot.Id, vehicle = vehicle.Plate });
    }

    [HttpPost("release/{spotId}")]
    public IActionResult Release(string spotId)
    {
        _service.Release(spotId);
        return Ok(new { message = $"Released {spotId}" });
    }

}