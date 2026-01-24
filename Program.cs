using HoneyRaesAPI.Models;
using HoneyRaesAPI.Models.DTOs;

List<Customer> customers = new List<Customer>
{
    new Customer { Id = 1, Name = "Robert Johnson", Address = "123 Maple Street, Nashville, TN 37203" },
    new Customer { Id = 2, Name = "Emily Davis", Address = "456 Oak Avenue, Franklin, TN 37064" },
    new Customer { Id = 3, Name = "Michael Brown", Address = "789 Pine Road, Brentwood, TN 37027" }
};
List<Employee> employees = new List<Employee> {
    new Employee { Id = 1, Name = "Sarah Mitchell", Specialty = "Plumbing" },
    new Employee { Id = 2, Name = "Marcus Chen", Specialty = "Electrical" },
    new Employee { Id = 3, Name = "Jessica Torres", Specialty = "HVAC" }};
List<ServiceTicket> serviceTickets = new List<ServiceTicket>
{
    new ServiceTicket { Id = 1, CustomerId = 1, EmployeeId = 1, Description = "Kitchen sink is leaking under the cabinet", Emergency = false, DateCompleted = new DateTime(2026, 1, 20) },
    new ServiceTicket { Id = 2, CustomerId = 2, EmployeeId = 2, Description = "Power outage in master bedroom", Emergency = true, DateCompleted = new DateTime(2026, 1, 22) },
    new ServiceTicket { Id = 3, CustomerId = 3, EmployeeId = 3, Description = "AC unit not cooling properly", Emergency = false, DateCompleted = new DateTime(2026, 1, 23) }
};

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ENDPOINTS
app.MapGet("/servicetickets", () =>
{
    return serviceTickets.Select(t => new ServiceTicketDTO
    {
        Id = t.Id,
        CustomerId = t.CustomerId,
        EmployeeId = t.EmployeeId,
        Description = t.Description,
        Emergency = t.Emergency,
        DateCompleted = t.DateCompleted
    });
});

app.MapGet("/servicetickets/{id}", (int id) =>
{

    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    return new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        EmployeeId = serviceTicket.EmployeeId,
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency,
        DateCompleted = serviceTicket.DateCompleted
    };

});

// LEFT OFF AT HONEY RAE CHAPTER 4
// ADDING ALL HONEY RAE GET ENDPOINTS

app.MapGet("/employees", () =>
{
    return employees.Select(e => new EmployeeDTO
    {
        Id = e.Id,
        Name = e.Name,
        Specialty = e.Specialty
    });
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);



    return new EmployeeDTO
    {
        Id = employee.Id,
        Name = employee.Name,
        Specialty = employee.Specialty
    };

});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

