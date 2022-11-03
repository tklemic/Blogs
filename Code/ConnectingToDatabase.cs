using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;


namespace Blogs
{
    class ConnectingToDatabase
    {
        public NpgsqlConnection conn;
        public void OpenConnection()
        {
            string connecting = "Server=localhost;" +
                                 "Port=5432;" +
                                 "User Id=postgres;" +
                                 "Password = postgres;" +
                                 "Database = Blogs;";
            conn = new NpgsqlConnection(connecting);
            conn.Open();
        }

        public void CloseConnection()
        {
            conn.Close();
        }
    }
}
