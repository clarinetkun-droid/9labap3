using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace _9labap3
{
    public static class DatabaseHelper
    {
        private static string dbPath = "siz_data.db";
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        public static void Initialize()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Employees (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        Position TEXT NOT NULL,
                        Department TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS SIZList (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Size TEXT,
                        WearPeriodMonths INTEGER
                    );
                    CREATE TABLE IF NOT EXISTS SIZInUse (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        EmployeeId INTEGER,
                        SizId INTEGER,
                        Quantity INTEGER,
                        IssueDate TEXT,
                        IsActive INTEGER DEFAULT 1
                    );";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Начальные данные (только при первом запуске)
                string seed = @"
                    INSERT INTO SIZList (Name, Size, WearPeriodMonths) 
                    SELECT 'Каска защитная', '54-60', 24 
                    WHERE NOT EXISTS (SELECT 1 FROM SIZList WHERE Name='Каска защитная');
                    
                    INSERT INTO SIZList (Name, Size, WearPeriodMonths) 
                    SELECT 'Перчатки х/б', '10', 1 
                    WHERE NOT EXISTS (SELECT 1 FROM SIZList WHERE Name='Перчатки х/б');
                    
                    INSERT INTO SIZList (Name, Size, WearPeriodMonths) 
                    SELECT 'Костюм сварщика', '48-56', 6 
                    WHERE NOT EXISTS (SELECT 1 FROM SIZList WHERE Name='Костюм сварщика');
                    
                    INSERT INTO Employees (FullName, Position, Department) 
                    SELECT 'Иванов Иван Иванович', 'Сварщик', 'Цех №1' 
                    WHERE NOT EXISTS (SELECT 1 FROM Employees WHERE FullName='Иванов Иван Иванович');";
                using (var cmd = new SQLiteCommand(seed, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ========== СОТРУДНИКИ ==========
        public static List<Employee> GetEmployees()
        {
            var list = new List<Employee>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Id, FullName, Position, Department FROM Employees", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Employee
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FullName = reader["FullName"].ToString(),
                            Position = reader["Position"].ToString(),
                            Department = reader["Department"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public static void AddEmployee(string fullName, string position, string department)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO Employees (FullName, Position, Department) VALUES (@n, @p, @d)", conn))
                {
                    cmd.Parameters.AddWithValue("@n", fullName);
                    cmd.Parameters.AddWithValue("@p", position);
                    cmd.Parameters.AddWithValue("@d", department);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateEmployee(int id, string fullName, string position, string department)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "UPDATE Employees SET FullName=@n, Position=@p, Department=@d WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@n", fullName);
                    cmd.Parameters.AddWithValue("@p", position);
                    cmd.Parameters.AddWithValue("@d", department);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteEmployee(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Employees WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ========== СИЗ ==========
        public static List<SIZ> GetSIZList()
        {
            var list = new List<SIZ>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Id, Name, Size, WearPeriodMonths FROM SIZList", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SIZ
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Size = reader["Size"].ToString(),
                            WearPeriodMonths = Convert.ToInt32(reader["WearPeriodMonths"])
                        });
                    }
                }
            }
            return list;
        }

        // ========== ВЫДАЧА ==========
        public static void IssueSIZ(int employeeId, int sizId, int quantity)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO SIZInUse (EmployeeId, SizId, Quantity, IssueDate, IsActive) VALUES (@e, @s, @q, @d, 1)", conn))
                {
                    cmd.Parameters.AddWithValue("@e", employeeId);
                    cmd.Parameters.AddWithValue("@s", sizId);
                    cmd.Parameters.AddWithValue("@q", quantity);
                    cmd.Parameters.AddWithValue("@d", DateTime.Now.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<SIZInUse> GetIssuedSIZ()
        {
            var list = new List<SIZInUse>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT i.Id, e.FullName, s.Name, i.Quantity, i.IssueDate, i.IsActive 
                               FROM SIZInUse i 
                               JOIN Employees e ON i.EmployeeId = e.Id 
                               JOIN SIZList s ON i.SizId = s.Id 
                               ORDER BY i.Id DESC";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SIZInUse
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            EmployeeName = reader["FullName"].ToString(),
                            SIZName = reader["Name"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            IssueDate = reader["IssueDate"].ToString(),
                            IsActive = Convert.ToInt32(reader["IsActive"]) == 1
                        });
                    }
                }
            }
            return list;
        }

        public static void WriteOffSIZ(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE SIZInUse SET IsActive=0 WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}