using Microsoft.AspNetCore.Mvc;

namespace Diplom2.Models
{
    public class StudentModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? MiddleName { get; set; }
        public int Age { get; set; }
        public string? Group { get; set; }
    }
}
