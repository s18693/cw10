using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using cw3.Models;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        public Enrollment GetEnrollment();
        public void AddStudnet(AddStudentRequest request);
        void PromoteStudents(PromoteStudents promote);
        public int getMsg();
    }

    public class AddStudentRequest
    {
        [Required(ErrorMessage = "Need student index like sXXXXX")]
        public string IndexNumber { get; set; }

        [Required(ErrorMessage = "Put First Name")]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Put Last Name")]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Need birth date")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Need name of studies")]
        public string Studies { get; set; }
    }

    public class PromoteStudents
    {
        public string studies { get; set; }

        public int semester { get; set; }
    }

    public class Enrollment
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public int IdStudy { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class LoginRequest
    {
        public string login { get; set; }
        public string password { get; set; }
    }
}
