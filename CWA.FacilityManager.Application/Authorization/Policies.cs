namespace CWA.FacilityManager.Application.Authorization
{
    /// <summary>
    /// Authorization policy constants
    /// </summary>
    public static class Policies
    {
        // Role-based policies
        public const string AdministratorOnly = "AdministratorOnly";
        public const string SecretaryOrHigher = "SecretaryOrHigher";
        public const string RenterOrHigher = "RenterOrHigher";

        // Feature-based policies
        public const string CanManageUsers = "CanManageUsers";
        public const string CanManageRoles = "CanManageRoles";
        public const string CanManageEvents = "CanManageEvents";
        public const string CanApproveEvents = "CanApproveEvents";
        public const string CanManageRooms = "CanManageRooms";
        public const string CanManageBuildings = "CanManageBuildings";
        public const string CanViewAllReservations = "CanViewAllReservations";
        public const string CanManageReservations = "CanManageReservations";
        public const string CanCreateReservations = "CanCreateReservations";
        public const string CanViewReports = "CanViewReports";
        public const string CanManageSystem = "CanManageSystem";
    }

    /// <summary>
    /// Role names constants
    /// </summary>
    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Secretary = "Secretary";
        public const string Renter = "Renter";
    }

    /// <summary>
    /// Role priority levels
    /// </summary>
    public static class RoleLevels
    {
        public const int Administrator = 100;
        public const int Secretary = 50;
        public const int Renter = 10;
    }
}
