using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repositories;

namespace StudentAdminPortal.API.Controllers
{
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository studentRepo;
        private readonly IMapper mapper;
        public StudentsController(IStudentRepository studentRepo, IMapper mapper) 
        {
            this.studentRepo = studentRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("[controller]")]
        public async Task<IActionResult> GetAllStudentsAsync()
        {
           var students = await studentRepo.GetStudentsAsync();

           return Ok(mapper.Map<List<StudentDTO>>(students));
        }
    }
}
