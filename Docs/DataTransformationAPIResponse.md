Data Transformation (Raw DB → API Response)
🔹 Step-by-Step Explanation
1. Raw DB (Normalized Data)
Flights Table:
Id | FlightNumber | Status
1  | AI101        | A
2. SQL Layer (Filtering + Pagination)

Handled in repository:

.OrderBy(f => f.Id)
.Skip(...)
.Take(...)
3. Service Layer (Transformation)
flights.Select(f => new FlightDto
{
    Id = f.Id,
    FlightNumber = f.FlightNumber,
    Status = f.Status == "A" ? "Active" : "Inactive"
})
4. API Response
[
  {
    "id": 1,
    "flightNumber": "AI101",
    "status": "Active"
  }
]
🧠 How to Explain This in Interview (IMPORTANT)

Say this smoothly:

“We fetch normalized data from the database using optimized queries with pagination. 
Then in the service layer, we transform that data into DTOs by applying business logic such as status mapping. 
Finally, we return a clean API response tailored for client consumption..”