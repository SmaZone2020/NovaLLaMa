using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace ClassLibrary
{
    public static class ConfigIO
    {
        private static readonly string _dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ndbf");

        public static void Initialize()
        {
            // 创建数据库文件
            if (!File.Exists(_dbFilePath))
                SQLiteConnection.CreateFile(_dbFilePath);

            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();

                // 创建 Models 表
                string createModelsTable = @"
                CREATE TABLE IF NOT EXISTS Models (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Param INTEGER NOT NULL,
                    AddDate TEXT NOT NULL,
                    FilePath TEXT NOT NULL,
                    Setting TEXT
                );";
                ExecuteNonQuery(connection, createModelsTable);

                // 创建 Setting 表
                string createSettingTable = @"
                CREATE TABLE IF NOT EXISTS Setting (
                    InstallPath TEXT NOT NULL,
                    Version TEXT NOT NULL
                );";
                ExecuteNonQuery(connection, createSettingTable);

                // 创建 Sessions 表
                string createSessionsTable = @"
                CREATE TABLE IF NOT EXISTS Sessions (
                    Name TEXT NOT NULL,
                    CreatDate TEXT NOT NULL,
                    ModelName TEXT NOT NULL,
                    ID TEXT NOT NULL,
                    AssistantName TEXT NOT NULL,
                    Content TEXT NOT NULL
                );";
                ExecuteNonQuery(connection, createSessionsTable);

                // 检查 Setting 表中是否有数据
                string checkSetting = "SELECT COUNT(*) FROM Setting;";
                using (var cmd = new SQLiteCommand(checkSetting, connection))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        // 如果 Setting 表没有数据，插入默认值
                        string insertDefaultSetting = @"
                        INSERT INTO Setting (InstallPath, Version) 
                        VALUES (@InstallPath, '1.0.0');";

                        using (var insertCmd = new SQLiteCommand(insertDefaultSetting, connection))
                        {
                            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NOVAModels");
                            insertCmd.Parameters.AddWithValue("@InstallPath", defaultPath);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
                var path = GetInstallPath();
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }

        public static List<Model> GetModelList()
        {
            var modelList = new List<Model>();
            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT * FROM Models;";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            modelList.Add(new Model
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                                Param = Convert.ToInt32(reader["Param"]),
                                AddDate = reader["AddDate"].ToString(),
                                FilePath = reader["FilePath"].ToString(),
                                Setting = reader["Setting"].ToString()
                            });
                        }
                    }
                }
            }
            return modelList;
        }

        public static void SetInstallPath(string installPath)
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();
                string updateQuery = "UPDATE Setting SET InstallPath = @InstallPath;";
                using (var cmd = new SQLiteCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@InstallPath", installPath);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static string GetInstallPath()
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT InstallPath FROM Setting LIMIT 1;";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string query)
        {
            using (var cmd = new SQLiteCommand(query, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddModel(string name, int param, string addDate, string filePath, string setting)
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();
                string insertQuery = @"
                INSERT INTO Models (Name, Param, AddDate, FilePath, Setting)
                VALUES (@Name, @Param, @AddDate, @FilePath, @Setting);";

                using (var cmd = new SQLiteCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Param", param);
                    cmd.Parameters.AddWithValue("@AddDate", addDate);
                    cmd.Parameters.AddWithValue("@FilePath", filePath);
                    cmd.Parameters.AddWithValue("@Setting", setting);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteModel(int id)
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Models WHERE ID = @ID;";
                using (var cmd = new SQLiteCommand(deleteQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddSession(string name, string creatDate, string modelName, string id,string assistantName, string setting = "[]")
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = "INSERT INTO Sessions (Name, CreatDate, ModelName, ID, AssistantName, Content) VALUES (@Name, @CreatDate, @ModelName, @ID, @AssistantName, @Content)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@CreatDate", creatDate);
                    command.Parameters.AddWithValue("@ModelName", modelName);
                    command.Parameters.AddWithValue("@ID", id); 
                        command.Parameters.AddWithValue("@AssistantName", assistantName);
                    command.Parameters.AddWithValue("@Content", $"[{{\"role\":\"system\",\"content\":\"{setting}\"}}]");

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateSessionContent(string id, string newContent)
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = "UPDATE Sessions SET Content = @Content WHERE ID = @ID";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.Parameters.AddWithValue("@Content", newContent);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteSession(string id)
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = "DELETE FROM Sessions WHERE ID = @ID";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Session> GetAllSessions()
        {
            List<Session> sessions = new List<Session>();

            using (var connection = new SQLiteConnection($"Data Source={_dbFilePath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT Name, CreatDate, ModelName, ID ,AssistantName, Content FROM Sessions";

                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var session = new Session(reader["Name"].ToString())
                        {
                            Name = reader["Name"].ToString(),
                            CreatDate = reader["CreatDate"].ToString(),
                            ModelName = reader["ModelName"].ToString(),
                            ID = reader["ID"].ToString(),
                            AssistantName = reader["AssistantName"].ToString(),
                            Chats = JsonConvert.DeserializeObject<ChatHistory[]>(reader["Content"].ToString())
                        };

                        sessions.Add(session);
                    }
                }
            }

            return sessions;
        }
    }

    public class Model
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Param { get; set; }
        public string AddDate { get; set; }
        public string FilePath { get; set; }
        public string Setting { get; set; }
    }
}
