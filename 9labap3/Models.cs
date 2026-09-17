namespace _9labap3
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
    }

    public class SIZ
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public int WearPeriodMonths { get; set; }
    }

    public class SIZInUse
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string SIZName { get; set; }
        public int Quantity { get; set; }
        public string IssueDate { get; set; }
        public bool IsActive { get; set; }
    }
}