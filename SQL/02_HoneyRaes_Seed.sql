\c HoneyRaes

INSERT INTO Customer ( Name, Address) VALUES 
    ('Robert Johnson', '123 Maple Street, Nashville, TN 37203' ),
    ('Emily Davis', '456 Oak Avenue, Franklin, TN 37064' ),
    ('Michael Brown', '789 Pine Road, Brentwood, TN 37027' );

INSERT INTO Employee ( Name, Specialty) VALUES 
    ('Sarah Mitchell', 'Plumbing' ),
    ('Marcus Chen', 'Electrical' ),
    ('Jessica Torres', 'HVAC' );

INSERT INTO ServiceTicket ( CustomerId, EmployeeId, Description, Emergency, DateCompleted) VALUES 
    (1, 1, 'Kitchen sink is leaking under the cabinet', false, '2026-01-20'),
    (2, 2, 'Power outage in master bedroom', true, '2026-01-22'),
    (3, 3, 'AC unit not cooling properly', false, '2026-01-23'),
    (3, NULL, 'AC unit not cooling properly', false, '2026-01-23');