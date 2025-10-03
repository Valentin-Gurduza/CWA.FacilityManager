# CWA Facility Manager

A comprehensive facility management system built with ASP.NET Core 9, Blazor WebAssembly, and SQL Server. This application manages room bookings, events, user roles, and provides a complete audit trail for all critical operations.

## Features

### User Management & Authorization
- **Three-tier role system**: Administrator (Level 100), Secretary (Level 50), Renter (Level 10)
- Role-based authorization with policy-based access control
- User creation, deactivation/reactivation, and role assignment
- Activity history and audit logging for all users
- Password reset and user profile management

### Event Management
- Create, edit, and delete events
- **Approval workflow**: Events start as "Pending" and require Secretary/Admin approval
- Conflict detection to prevent double-booking
- Event statuses: Pending, Approved, Rejected, Cancelled
- Comprehensive event details: organizer company, contact information, attendee count

### Room Management
- Room CRUD operations with building associations
- Room properties: capacity, amenities (projector, microphone, etc.), availability
- Availability checking for specific time ranges
- Filter rooms by capacity, building, and availability

### Calendar & Booking
- Global calendar view for all approved events
- Personal calendar showing user's own reservations
- Detailed event information display with room and building details
- Time conflict detection and resolution

### Audit Logging
- Track all critical actions (create, update, delete, approve, reject)
- User action history with timestamps, IP addresses, and user agents
- Entity-specific audit trails

## Technology Stack

- **Backend**: ASP.NET Core 9.0
- **Frontend**: Blazor WebAssembly + Blazor Server
- **Database**: SQL Server with Entity Framework Core 9.0
- **Authentication**: ASP.NET Core Identity
- **Authorization**: Policy-based with custom role hierarchy handlers

## Project Structure

```
CWA.FacilityManager/
├── CWA.FacilityManager.Server/          # Backend server and API
│   ├── Controllers/                      # API controllers
│   ├── Components/                       # Blazor Server components
│   └── Program.cs                        # Application startup
├── CWA.FacilityManager.Client/          # Blazor WebAssembly client
│   └── Pages/                            # Client-side pages
├── CWA.FacilityManager.Application/     # Business logic layer
│   ├── Services/                         # Service implementations
│   ├── Interfaces/                       # Service interfaces
│   └── Authorization/                    # Authorization policies
├── CWA.FacilityManager.Domain/          # Domain models
│   ├── Models/                           # Entity models
│   └── Enums/                            # Enumerations
├── CWA.FacilityManager.Infrastructure/  # Data access layer
│   ├── Contexts/                         # Database contexts
│   └── Migrations/                       # EF Core migrations
└── CWA.FacilityManager.Shared/          # Shared code
```

## Database Schema

### Key Entities

#### ApplicationUser
- Extended ASP.NET Identity user with custom properties
- FirstName, LastName, Department, Position, IsActive
- Relationships to Bookings, Events, AuditLogs, UserHistory

#### ApplicationRole
- Extended ASP.NET Identity role with hierarchy
- Priority/Level field (100 for Admin, 50 for Secretary, 10 for Renter)
- RoleType enum and system role flags

#### Room
- Name, RoomNumber, Capacity, Description
- Amenities (JSON/CSV list of facilities)
- Building relationship
- Date and Time for scheduling
- Equipment and Activity type

#### Event
- Title, Description, Type (Meeting, Course, Conference, Training, Workshop)
- StartDateTime, EndDateTime
- OrganizerCompany, ContactName, ContactPhone
- Status: Pending, Approved, Rejected, Cancelled
- ApprovedBy and ApprovedAt for audit trail

#### Building
- Name, Address, Description
- Collection of Rooms

#### AuditLog
- UserId, ActionType, Entity, EntityId
- Data (JSON), IpAddress, UserAgent
- Timestamp

## Setup Instructions

### Prerequisites

- .NET 9.0 SDK
- SQL Server (or SQL Express)
- Visual Studio 2022 or VS Code with C# extension

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/Valentin-Gurduza/CWA.FacilityManager.git
   cd CWA.FacilityManager
   ```

2. **Configure database connection**
   
   Update the connection string in `CWA.FacilityManager.Server/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=FacilityManager;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Install EF Core tools** (if not already installed)
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Apply database migrations**
   ```bash
   cd CWA.FacilityManager
   dotnet ef database update --project CWA.FacilityManager.Infrastructure --startup-project CWA.FacilityManager.Server
   ```

5. **Build the solution**
   ```bash
   dotnet build
   ```

6. **Run the application**
   ```bash
   dotnet run --project CWA.FacilityManager.Server
   ```

7. **Access the application**
   - Open browser to `https://localhost:5001` (or the port shown in console)
   - Default administrator credentials:
     - **Email**: `admin@facilitymanager.local`
     - **Password**: `Admin@123`
   - **IMPORTANT**: Change the default password after first login!

## Default Credentials

The system automatically creates a default administrator account on first run:

- **Email**: admin@facilitymanager.local
- **Username**: admin
- **Password**: Admin@123

**Security Note**: This default account is created for initial setup only. You should:
1. Log in immediately after first deployment
2. Change the administrator password
3. Create additional administrator accounts if needed
4. Consider deactivating the default admin account after creating replacement administrators

## Database Migrations

### Create a new migration
```bash
dotnet ef migrations add MigrationName --project CWA.FacilityManager.Infrastructure --startup-project CWA.FacilityManager.Server
```

### Apply migrations
```bash
dotnet ef database update --project CWA.FacilityManager.Infrastructure --startup-project CWA.FacilityManager.Server
```

### Remove last migration
```bash
dotnet ef migrations remove --project CWA.FacilityManager.Infrastructure --startup-project CWA.FacilityManager.Server
```

### Generate SQL script
```bash
dotnet ef migrations script --project CWA.FacilityManager.Infrastructure --startup-project CWA.FacilityManager.Server --output migration.sql
```

## API Endpoints

### Rooms
- `GET /api/rooms` - Get all rooms (Requires: Renter or higher)
- `GET /api/rooms/{id}` - Get room by ID (Requires: Renter or higher)
- `GET /api/rooms/building/{buildingId}` - Get rooms by building (Requires: Renter or higher)
- `GET /api/rooms/available` - Get available rooms for time slot (Requires: Renter or higher)
- `POST /api/rooms` - Create room (Requires: Secretary or higher)
- `PUT /api/rooms/{id}` - Update room (Requires: Secretary or higher)
- `DELETE /api/rooms/{id}` - Delete room (Requires: Secretary or higher)

### Events
- `GET /api/events` - Get all events (Requires: Secretary or higher)
- `GET /api/events/{id}` - Get event by ID (Requires: Renter or higher, own events only)
- `GET /api/events/date-range` - Get events by date range (Requires: Renter or higher)
- `GET /api/events/room/{roomId}` - Get events by room (Requires: Renter or higher)
- `GET /api/events/pending` - Get pending events (Requires: Secretary or higher)
- `GET /api/events/my-events` - Get current user's events (Requires: Renter or higher)
- `GET /api/events/check-availability` - Check room availability (Requires: Renter or higher)
- `POST /api/events` - Create event (Requires: Renter or higher)
- `PUT /api/events/{id}` - Update event (Requires: Owner or Secretary or higher)
- `POST /api/events/{id}/approve` - Approve event (Requires: Secretary or higher)
- `POST /api/events/{id}/reject` - Reject event (Requires: Secretary or higher)
- `POST /api/events/{id}/cancel` - Cancel event (Requires: Owner or Secretary or higher)
- `DELETE /api/events/{id}` - Delete event (Requires: Secretary or higher)

## Authorization Policies

### Role-Based Policies
- **AdministratorOnly** - Requires level 100 (Administrator)
- **SecretaryOrHigher** - Requires level 50+ (Secretary or Administrator)
- **RenterOrHigher** - Requires level 10+ (Any authenticated user)

### Feature-Based Policies
- **CanManageUsers** - User management operations (Administrator only)
- **CanManageRoles** - Role management operations (Administrator only)
- **CanManageEvents** - Event CRUD operations (Secretary or higher)
- **CanApproveEvents** - Event approval/rejection (Secretary or higher)
- **CanManageRooms** - Room CRUD operations (Secretary or higher)
- **CanManageBuildings** - Building CRUD operations (Administrator only)
- **CanViewAllReservations** - View all reservations (Secretary or higher)
- **CanManageReservations** - Manage reservations (Secretary or higher)
- **CanCreateReservations** - Create new reservations (Renter or higher)
- **CanViewReports** - View reports (Secretary or higher)
- **CanManageSystem** - System configuration (Administrator only)

## Testing

### Run all tests
```bash
dotnet test
```

### Run tests for a specific project
```bash
dotnet test CWA.FacilityManager.Tests
```

### Run tests with coverage
```bash
dotnet test /p:CollectCoverage=true
```

## Seeded Data

The application automatically seeds the following data on first run:

### Roles
- **Administrator** (Priority: 100)
  - Full system access
  - User and role management
  - System configuration

- **Secretary** (Priority: 50)
  - Event management and approval
  - Room management
  - View all calendars and reports

- **Renter** (Priority: 10)
  - Create reservation requests
  - View own calendar and history
  - View available rooms

### Permissions
The system seeds comprehensive permissions across modules:
- User Management (View, Create, Edit, Delete, Assign, Unassign)
- Role Management (View, Create, Edit, Delete, AssignPermissions)
- Room Management (View, Create, Edit, Delete, ViewSchedule, ManageSchedule)
- Event Management (View, Create, Edit, Delete, Approve, Reject, Cancel)
- Booking Management (View, Create, Edit, Delete, ViewAll, Approve, Reject)
- Report Management (View, Generate, Export)
- System Configuration (View, Edit, Backup, Restore)

## Business Rules

### Event Creation
1. Events start with "Pending" status
2. Conflict detection prevents double-booking
3. Events require Secretary/Admin approval before becoming "Approved"
4. Renters can only create reservation requests
5. Secretary/Admin can create approved events directly

### Event Modification
- Renters can only edit their own pending events
- Secretary/Admin can edit any event
- Changing room or time triggers conflict check
- Approved events require re-approval after modification

### Room Availability
- Only active rooms are bookable
- Rooms with capacity constraints are filtered automatically
- Conflicting events (except Rejected/Cancelled) block availability

### Authorization
- All API endpoints require authentication
- Role hierarchy is enforced (higher level includes lower level permissions)
- Audit log records all critical actions

## Troubleshooting

### Database connection issues
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database user has appropriate permissions

### Migration errors
- Delete database and rerun migrations: `dotnet ef database drop`
- Check for conflicts in migration files
- Ensure all projects are building successfully

### Build errors
- Clean solution: `dotnet clean`
- Restore packages: `dotnet restore`
- Rebuild: `dotnet build`

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, questions, or contributions, please create an issue in the GitHub repository.

## Rollback Instructions

### Rollback Database Migration
To revert to a previous migration:
```bash
dotnet ef database update PreviousMigrationName --project CWA.FacilityManager.Infrastructure --startup-project CWA.FacilityManager.Server
```

### Rollback Code Changes
```bash
git revert <commit-hash>
git push origin main
```

## Development Notes

- Use consistent naming conventions
- Follow SOLID principles
- Add XML comments to public APIs
- Include unit tests for business logic
- Use async/await for all database operations
- Implement proper error handling and logging
- Validate input on both client and server
