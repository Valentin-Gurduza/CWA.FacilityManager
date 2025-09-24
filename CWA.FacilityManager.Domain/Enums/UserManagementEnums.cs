namespace CWA.FacilityManager.Domain.Enums
{
    public enum ModuleType
    {
        UserManagement,
        RoleManagement,
        FacilityManagement,
        AssetManagement,
        MaintenanceManagement,
        ReportsAndAnalytics,
        SystemAdministration,
        EventManagement,
        BookingManagement,
        ReportManagement,
        SystemManagement
    }

    public enum ResourceType
    {
        Users,
        Roles,
        Permissions,
        Facilities,
        Rooms,
        Assets,
        Events,
        Bookings,
        WorkOrders,
        Reports,
        SystemSettings,
        Configuration,
        AuditLogs
    }

    public enum ActionType
    {
        View,
        Create,
        Edit,
        Delete,
        Assign,
        Unassign,
        Approve,
        Reject,
        Cancel,
        Export,
        Import,
        Archive,
        Restore,
        ViewAll,
        ViewSchedule,
        ManageSchedule,
        AssignPermissions,
        Generate,
        Backup
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Pending,
        Suspended,
        Archived
    }

    public enum RoleType
    {
        System,
        Custom,
        Administrator,
        Secretary,
        Renter,
        FacilityManager,
        MaintenanceManager,
        Technician,
        Viewer
    }
}