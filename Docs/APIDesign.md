API Design (Simple Implementation)
🔹 Scenario

👉 Build API to fetch flight data with pagination

✅ Controller
[ApiController]
[Route("api/[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightsController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFlights(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _flightService.GetFlightsAsync(pageNumber, pageSize);
        return Ok(result);
    }
}
✅ Service Layer
public interface IFlightService
{
    Task<List<FlightDto>> GetFlightsAsync(int pageNumber, int pageSize);
}

public class FlightService : IFlightService
{
    private readonly IFlightRepository _repository;

    public FlightService(IFlightRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<FlightDto>> GetFlightsAsync(int pageNumber, int pageSize)
    {
        var flights = await _repository.GetFlightsAsync(pageNumber, pageSize);

        // Transformation (Entity → DTO)
        return flights.Select(f => new FlightDto
        {
            Id = f.Id,
            FlightNumber = f.FlightNumber,
            Status = f.Status == "A" ? "Active" : "Inactive"
        }).ToList();
    }
}
✅ Repository Layer
public interface IFlightRepository
{
    Task<List<Flight>> GetFlightsAsync(int pageNumber, int pageSize);
}

public class FlightRepository : IFlightRepository
{
    private readonly AppDbContext _context;

    public FlightRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Flight>> GetFlightsAsync(int pageNumber, int pageSize)
    {
        return await _context.Flights
            .OrderBy(f => f.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
✅ DTO (API Contract)
public class FlightDto
{
    public int Id { get; set; }
    public string FlightNumber { get; set; }
    public string Status { get; set; }
}
🔥 What to SAY while explaining

“We follow a layered architecture where the controller handles requests, the service layer applies business logic and transformation, and the repository handles data access with pagination.”