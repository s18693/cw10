using System;
using Microsoft.AspNetCore.Mvc;
using cw3.Models;
using cw3.DAL;
using System.Data.SqlClient;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        [HttpGet("sql/get")]
        public IActionResult GetStudents(string index)
        {

            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = $"select Student.FirstName, Student.LastName, Enrollment.Semester from Student inner join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment where Student.IndexNumber =@index";
                com.Parameters.AddWithValue("index", index);

                client.Open();
                var dr = com.ExecuteReader();

                while (dr.Read())
                {
                    var st = new Student { IndexNumber = index, FirstName = dr["FirstName"].ToString(), LastName = dr["LastName"].ToString() };
                    return Ok($"{dr.HasRows} {st.ToString()} {dr["Semester"].ToString()} ");
                }
                return NotFound();
            }

            //return Ok(_dbService.GetStudents());

        }

        [HttpPost("sql/crt {id}")]
        public IActionResult CreateStudent(Student student, string studies)
        {
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18693;Integrated Security=True"))
            using (var command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = "select StudiesN.IdStudy from StudentN where StudiesN.Name = @S; ";
                command.Parameters.AddWithValue("S", studies);
                client.Open();
                var dr = command.ExecuteReader();
                int eId = -1;
                while (dr.Read())
                {
                    eId = int.Parse(dr["IdStudy"].ToString());
                }
                client.Close();
                command.CommandText = "Insert into Student values (@index,@Fname,@Lname,@Date," + eId + ")";
                command.Parameters.AddWithValue("index", student.IndexNumber);
                command.Parameters.AddWithValue("Fname", student.FirstName);
                command.Parameters.AddWithValue("Lname", student.LastName);
                command.Parameters.AddWithValue("Date", student.BirthDate);
                command.Parameters.AddWithValue("E", student.IdEnrollment);

                client.Open();
                command.ExecuteNonQuery();
            }
            return StatusCode((int)System.Net.HttpStatusCode.Created);
            //return BadRequest();400
            //return Createrd("", obiekt pobrany z bazy);
            //return StatusCode((int)HttpStatuseCode.Created, objekt);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateStudetn(int id)
        {

            return Ok("Aktualizacja dokonczona");
        }
        [HttpDelete]
        public IActionResult DeleteStudent(int id)
        {

            return Ok("Usuwanie ukonczone");
        }

       
    }
}