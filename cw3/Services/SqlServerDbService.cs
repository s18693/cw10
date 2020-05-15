using System.Collections.Generic;
using System.Data.SqlClient;
using cw3.Models;
using System;

namespace cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        string SqlConnection = "Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True";
        int msg = -1;
        int lastUseE = -1;
        public void AddStudnet(AddStudentRequest request)
        {
            using (var client = new SqlConnection(SqlConnection))
            using (var command = new SqlCommand())
            {
                //standart operation client-connection

                command.Connection = client;
                client.Open();

                //transaction must be added to command
                var transaction = client.BeginTransaction();
                command.Transaction = transaction;
                try
                {

                    //Check study
                    command.CommandText = "select StudiesN.IdStudy from StudiesN where StudiesN.Name = @name";
                    command.Parameters.AddWithValue("name", request.Studies);

                    var dr = command.ExecuteReader();
                    if (!dr.Read())
                    {
                        //need to close
                        dr.Close();
                        transaction.Rollback();
                        msg = -2;
                        return;
                    }

                    msg = 0;
                    int id = (int)dr["IdStudy"];
                    dr.Close();
                    //Check Student index
                    command.CommandText = "select StudentN.IndexNumber from StudentN where StudentN.IndexNumber like '" + request.IndexNumber + "'";
                    var readerb = command.ExecuteReader();
                    if (readerb.Read())
                    {
                        readerb.Close();
                        transaction.Rollback();
                        msg = -3;
                        return;
                    }
                    readerb.Close();

                    //Check
                    int EId = -1;
                    command.CommandText = "select EnrollmentN.IdEnrollment from EnrollmentN where EnrollmentN.Semester = 1 and EnrollmentN.IdStudy =" + id;
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        EId = (int)reader["IdEnrollment"];
                        reader.Close();
                    }
                    if (!reader.IsClosed)
                        if (!reader.Read())
                        {

                            reader.Close();
                            //This table need all values
                            //Find the lowest free id
                            command.CommandText = "select coalesce(min(t1.IdEnrollment) + 1, 0) from EnrollmentN t1 left outer join EnrollmentN t2 on t1.IdEnrollment = t2.IdEnrollment - 1 where t2.IdEnrollment is null";
                            var readera = command.ExecuteReader();
                            if (readera.Read())
                                EId = (int)readera[0];
                            readera.Close();
                            DateTime date = DateTime.Now.Date;
                            string convertedDate = date.Month + "/" + date.Day + "/" + date.Year;

                            command.CommandText = "insert into EnrollmentN values(" + EId + ", 1," + id + " , '" + convertedDate + "')";
                            //command.CommandText = "insert into EnrollmentN values(" + EId + "1," + id + " '12/12/2015')";
                            command.ExecuteNonQuery();
                        }

                    //Add new student
                    lastUseE = EId;
                    command.CommandText = "Insert into StudentN values (@index,@Fname,@Lname,@Date," + EId + ")";
                    command.Parameters.AddWithValue("index", request.IndexNumber);
                    command.Parameters.AddWithValue("Fname", request.FirstName);
                    command.Parameters.AddWithValue("Lname", request.LastName);
                    command.Parameters.AddWithValue("Date", request.BirthDate);

                    //Can be close and open connection. no commit, need close reader
                    //command.Connection.Close();
                    //command.Connection.Open();
                    //
                    //dr.Close();

                    command.ExecuteNonQuery();
                    transaction.Commit();
                    //transaction.Rollback();
                    msg = 1;
                }
                catch (SqlException exc)
                {
                    transaction.Rollback();
                    msg = -4;
                }
            }
        }

        public void PromoteStudents(PromoteStudents promote)
        {
            using (var client = new SqlConnection(SqlConnection))
            using (var command = new SqlCommand())
            {
                command.Connection = client;
                client.Open();
                var transaction = client.BeginTransaction();
                command.Transaction = transaction;
                //try{
                    //@studies varchar(250), @semester int 

                    //Need id
                    int id = -1;
                    command.CommandText = "select EnrollmentN.IdEnrollment from EnrollmentN inner join StudiesN on EnrollmentN.IdStudy = StudiesN.IdStudy and StudiesN.Name like '" + promote.studies + "' where EnrollmentN.Semester =" + (promote.semester + 1);
                    var dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        lastUseE = (int)dr[0];
                        dr.Close();
                    }
                    if(!dr.Read())
                    {
                        dr.Close();
                        command.CommandText = "select * from findMinEnrollmentN";
                        var reader = command.ExecuteReader();
                        
                        if (reader.Read())
                        {
                            lastUseE = (int)reader[0];
                            reader.Close();
                        }
                        else
                        {
                            reader.Close();
                        }
                    }

                    command.CommandText = "promoteStudent";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("studies", promote.studies);
                    command.Parameters.AddWithValue("semester", promote.semester);

                    //string s += client.InfoMessage;

                    command.ExecuteNonQuery();
                    transaction.Commit();

                    msg = id;
                /*
                }
                catch (SqlException exc)
                {
                    transaction.Rollback();
                    msg = -4;
                    return;
                }*/
            }
        }

        public int getMsg()
        {
            return msg;
        }

        public Enrollment GetEnrollment()
        {
            using (var client = new SqlConnection(SqlConnection))
            using (var command = new SqlCommand())
            {
                command.Connection = client;
                client.Open();

                command.CommandText = "select * from EnrollmentN where EnrollmentN.IdEnrollment = " + lastUseE;

                var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    return new Enrollment { IdEnrollment = (int)dr["IdEnrollment"], Semester = (int)dr["Semester"], IdStudy = (int)dr["IdStudy"], StartDate = (DateTime)dr["StartDate"] };
                }
            }
                return null;
        }
    }
}
