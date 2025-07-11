namespace ServiceNow.Core.Constants;

public static class ServiceNowConstants
{
    // Table Names
    public const string IncidentTable = "incident";
    public const string ChangeRequestTable = "change_request";
    public const string UserTable = "sys_user";
    public const string CMDBBaseTable = "cmdb_ci";
    public const string CMDBServerTable = "cmdb_ci_server";
    
    // Incident States
    public static class IncidentState
    {
        public const int New = 1;
        public const int InProgress = 2;
        public const int OnHold = 3;
        public const int Resolved = 6;
        public const int Closed = 7;
        public const int Cancelled = 8;
    }
    
    // Change States
    public static class ChangeState
    {
        public const int New = -5;
        public const int Assess = -4;
        public const int Authorize = -3;
        public const int Scheduled = -2;
        public const int Implement = -1;
        public const int Review = 0;
        public const int Closed = 3;
        public const int Cancelled = 4;
    }
    
    // Priorities
    public static class Priority
    {
        public const int Critical = 1;
        public const int High = 2;
        public const int Moderate = 3;
        public const int Low = 4;
        public const int Planning = 5;
    }
    
    // Impact/Urgency
    public static class Impact
    {
        public const int High = 1;
        public const int Medium = 2;
        public const int Low = 3;
    }
}
