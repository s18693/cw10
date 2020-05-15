using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DAL
{
    public class DbService : IDbService
    {

        string SqlConnection = "Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True";
        public int msg = -1;

        public bool checkIndex(string index)
        {
            using (var client = new SqlConnection(SqlConnection))
            using (var command = new SqlCommand())
            {
                command.Connection = client;
                client.Open();

                try
                {

                    command.CommandText = "select * from StudentN where StudentN.IndexNumber like '" + index +"'";

                    var read = command.ExecuteReader();
                    if (read.Read())
                    {
                        read.Close();
                        msg = 1;
                        return true;
                    }
                    if(!read.Read())
                    {
                        read.Close();
                        msg = 2;
                        return false;
                    }

                }
                catch (SqlException exc)
                {
                    msg = -2;
                    return false;
                }

            }
            return false;
        }
    }
}
