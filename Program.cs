using HoneyRaesAPI.Models;
using HoneyRaesAPI.Models.DTOs;
using Npgsql;
using DotNetEnv;
using Sprache;

Env.Load();

var connectionString = Env.GetString("POSTGRES_CONNECT");

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

// // ENDPOINTS
// app.MapGet("/servicetickets", () =>
// {
//     return serviceTickets.Select(t => new ServiceTicketDTO
//     {
//         Id = t.Id,
//         CustomerId = t.CustomerId,
//         EmployeeId = t.EmployeeId,
//         Description = t.Description,
//         Emergency = t.Emergency,
//         DateCompleted = t.DateCompleted
//     });
// });

// app.MapGet("/servicetickets/{id}", (int id) =>
// {

//     ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

//     if (serviceTicket == null)
//     {
//         return Results.NotFound();
//     }
//     Employee employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);

//     Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);


//     return Results.Ok(new ServiceTicketDTO
//     {
//         Id = serviceTicket.Id,
//         CustomerId = serviceTicket.CustomerId,
//         Customer = customer == null ? null : new CustomerDTO
//         {
//             Id = customer.Id,
//             Name = customer.Name,
//             Address = customer.Address
//         },
//         EmployeeId = serviceTicket.EmployeeId,
//         Description = serviceTicket.Description,
//         Emergency = serviceTicket.Emergency,
//         DateCompleted = serviceTicket.DateCompleted,
//         Employee = employee == null ? null : new EmployeeDTO
//         {
//             Id = employee.Id,
//             Name = employee.Name,
//             Specialty = employee.Specialty,

//         }
//     });

// });

// app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
// {
//     // Get the customer data to check that the customerid for the service ticket is valid
//     Customer customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);

//     // If the client did not provide a valid customer id, this is a bad request
//     if (customer == null)
//     {
//         return Results.BadRequest();
//     }

//     // creates a new id (SQL will do this for us like JSON server)
//     serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
//     serviceTickets.Add(serviceTicket);

//     return Results.Created($"/servicetickets/{serviceTicket.Id}", new ServiceTicketDTO
//     {
//         Id = serviceTicket.Id,
//         CustomerId = serviceTicket.CustomerId,
//         Customer = new CustomerDTO
//         {
//             Id = customer.Id,
//             Name = customer.Name,
//             Address = customer.Address
//         },
//         Description = serviceTicket.Description,
//         Emergency = serviceTicket.Emergency
//     });
// });

// app.MapPost("/servicetickets/{id}/complete", (int id) =>
// {
//     ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);

//     if (serviceTicket == null)
//     {
//         return Results.NotFound();
//     }

//     serviceTicket.DateCompleted = DateTime.Today;
//     return Results.NoContent();
// });

// app.MapDelete("/servicetickets/{id}", (int id) =>
// {
//     ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
//     if (serviceTicket == null)
//     {
//         return Results.BadRequest();
//     }

//     serviceTickets.Remove(serviceTicket);
//     return Results.NoContent();
// });

// app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
// {
//     ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);

//     if (ticketToUpdate == null)
//     {
//         return Results.NotFound();
//     }

//     if (id != serviceTicket.Id)
//     {
//         return Results.BadRequest();
//     }

//     ticketToUpdate.CustomerId = serviceTicket.CustomerId;
//     ticketToUpdate.EmployeeId = serviceTicket.EmployeeId;
//     ticketToUpdate.Description = serviceTicket.Description;
//     ticketToUpdate.Emergency = serviceTicket.Emergency;
//     ticketToUpdate.DateCompleted = serviceTicket.DateCompleted;

//     return Results.NoContent();
// });

app.MapGet("/employees", () =>
{
    List<Employee> employees = new List<Employee>();

    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);

    connection.Open();

    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = "SELECT * FROM Employee";

    using NpgsqlDataReader reader = command.ExecuteReader();

    while (reader.Read())
    {
        employees.Add(new Employee
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
        });
    }
    return employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = null;
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        SELECT 
            e.Id,
            e.Name, 
            e.Specialty, 
            st.Id AS serviceTicketId, 
            st.CustomerId,
            st.Description,
            st.Emergency,
            st.DateCompleted 
        FROM Employee e
        LEFT JOIN ServiceTicket st ON st.EmployeeId = e.Id
        WHERE e.Id = @id";

    command.Parameters.AddWithValue("@id", id);

    using NpgsqlDataReader reader = command.ExecuteReader();

    while (reader.Read())
    {
        if (employee == null)
        {
            employee = new Employee
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                ServiceTickets = new List<ServiceTicket>()
            };
        }

        if (!reader.IsDBNull(reader.GetOrdinal("serviceTicketId")))
        {
            employee.ServiceTickets?.Add(new ServiceTicket
            {
                Id = reader.GetInt32(reader.GetOrdinal("serviceTicketId")),
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                EmployeeId = id,
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Emergency = reader.GetBoolean(reader.GetOrdinal("Emergency")),
                DateCompleted = reader.IsDBNull(reader.GetOrdinal("DateCompleted")) ? null : reader.GetDateTime(reader.GetOrdinal("DateCompleted"))
            });
        }
    }
    return employee == null ? Results.NotFound() : Results.Ok(employee);
});

app.MapPost("/employees", (Employee employee) =>
{
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);

    connection.Open();

    using NpgsqlCommand command = connection.CreateCommand();

    command.CommandText = @"
        INSERT INTO Employee (Name, Specialty)
        VALUES (@name, @specialty)
        RETURNING Id
    ";

    command.Parameters.AddWithValue("@name", employee.Name);
    command.Parameters.AddWithValue("@specialty", employee.Specialty);

    employee.Id = (int)command.ExecuteScalar();
    return employee;
});

app.MapPut("/employees/{id}", (int id, Employee employee) =>
{
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        UPDATE Employee
        SET Name = @name,
            Specialty = @specialty
        WHERE Id = @id
    ";

    command.Parameters.AddWithValue("@name", employee.Name);
    command.Parameters.AddWithValue("@specialty", employee.Specialty);
    command.Parameters.AddWithValue("@id", id);

    command.ExecuteNonQuery();
    return Results.NoContent();
});

app.MapDelete("/employees/{id}", (int id) =>
{
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        DELETE FROM Employee
        WHERE Id = @id
    ";

    command.Parameters.AddWithValue("@id", id);

    int rowsAffected = command.ExecuteNonQuery();
    return rowsAffected > 0 ? Results.NoContent() : Results.NotFound();
});

// app.MapGet("/customers", () =>
// {
//     return customers.Select(c => new CustomerDTO
//     {
//         Id = c.Id,
//         Name = c.Name,
//         Address = c.Address
//     });
// });

// app.MapGet("/customers/{id}", (int id) =>
// {
//     Customer customer = customers.FirstOrDefault(c => c.Id == id);
//     if (customer == null)
//     {
//         return Results.NotFound();
//     }

//     List<ServiceTicket> customerTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
//     return Results.Ok(new CustomerDTO
//     {
//         Id = customer.Id,
//         Name = customer.Name,
//         Address = customer.Address,
//         ServiceTickets = customerTickets.Select(ct =>
//         {
//             Employee employee = employees.FirstOrDefault(e => e.Id == ct.EmployeeId);


//             return new ServiceTicketDTO
//             {
//                 Id = ct.Id,
//                 CustomerId = ct.CustomerId,
//                 EmployeeId = ct.EmployeeId,
//                 Description = ct.Description,
//                 DateCompleted = ct.DateCompleted,
//                 Employee = employee == null ? null : new EmployeeDTO
//                 {
//                     Id = employee.Id,
//                     Name = employee.Name,
//                     Specialty = employee.Specialty
//                 }
//             };
//         }).ToList()
//     });


// });


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

