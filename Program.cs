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
    new ServiceTicket { Id = 3, CustomerId = 3, EmployeeId = 3, Description = "AC unit not cooling properly", Emergency = false, DateCompleted = new DateTime(2026, 1, 23) },
    new ServiceTicket { Id = 4, CustomerId = 3, Description = "AC unit not cooling properly", Emergency = false, DateCompleted = new DateTime(2026, 1, 23) }
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

    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    Employee employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);

    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);


    return Results.Ok(new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = customer == null ? null : new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        EmployeeId = serviceTicket.EmployeeId,
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency,
        DateCompleted = serviceTicket.DateCompleted,
        Employee = employee == null ? null : new EmployeeDTO
        {
            Id = employee.Id,
            Name = employee.Name,
            Specialty = employee.Specialty,

        }
    });

});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // Get the customer data to check that the customerid for the service ticket is valid
    Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

    // If the client did not provide a valid customer id, this is a bad request
    if (customer == null)
    {
        return Results.BadRequest();
    }

    // creates a new id (SQL will do this for us like JSON server)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);

    return Results.Created($"/servicetickets/{serviceTicket.Id}", new ServiceTicketDTO
    {
        Id = serviceTicket.Id,
        CustomerId = serviceTicket.CustomerId,
        Customer = new CustomerDTO
        {
            Id = customer.Id,
            Name = customer.Name,
            Address = customer.Address
        },
        Description = serviceTicket.Description,
        Emergency = serviceTicket.Emergency
    });
});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (serviceTicket == null)
    {
        return Results.NotFound();
    }

    serviceTicket.DateCompleted = DateTime.Today;
    return Results.NoContent();
});

app.MapDelete("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.BadRequest();
    }

    serviceTickets.Remove(serviceTicket);
    return Results.NoContent();
});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);

    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }

    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }

    ticketToUpdate.CustomerId = serviceTicket.CustomerId;
    ticketToUpdate.EmployeeId = serviceTicket.EmployeeId;
    ticketToUpdate.Description = serviceTicket.Description;
    ticketToUpdate.Emergency = serviceTicket.Emergency;
    ticketToUpdate.DateCompleted = serviceTicket.DateCompleted;

    return Results.NoContent();
});

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
    List<ServiceTicket> tickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();

    if (employee == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new EmployeeDTO
    {
        Id = employee.Id,
        Name = employee.Name,
        Specialty = employee.Specialty,
        ServiceTickets = tickets.Select(t => new ServiceTicketDTO
        {
            Id = t.Id,
            CustomerId = t.CustomerId,
            EmployeeId = t.EmployeeId,
            Description = t.Description,
            Emergency = t.Emergency,
            DateCompleted = t.DateCompleted
        }).ToList()
    });

});

app.MapGet("/customers", () =>
{
    return customers.Select(c => new CustomerDTO
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
});

app.MapGet("/customers/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }

    List<ServiceTicket> customerTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return Results.Ok(new CustomerDTO
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address,
        ServiceTickets = customerTickets.Select(ct =>
        {
            Employee employee = employees.FirstOrDefault(e => e.Id == ct.EmployeeId);


            return new ServiceTicketDTO
            {
                Id = ct.Id,
                CustomerId = ct.CustomerId,
                EmployeeId = ct.EmployeeId,
                Description = ct.Description,
                DateCompleted = ct.DateCompleted,
                Employee = employee == null ? null : new EmployeeDTO
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Specialty = employee.Specialty
                }
            };
        }).ToList()
    });


});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

