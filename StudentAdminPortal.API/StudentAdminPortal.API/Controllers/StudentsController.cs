using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DataModels;
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

        [HttpGet]
        [Route("[controller]/{studentId:guid}"), ActionName("GetStudentAsync")]
        public async Task<IActionResult> GetStudentAsync([FromRoute] Guid studentId)
        {
            //Fetch single student detail
            var student = await studentRepo.GetStudentAsync(studentId);

            if (student == null) return NotFound();

            return Ok(mapper.Map<StudentDTO>(student));
        }
        [HttpPut]
        [Route("[controller]/{studentId:guid}")]
        public async Task<IActionResult> UpdateStudentAsync([FromRoute] Guid studentId, [FromBody] UpdateStudentRequest request)
        {
            if (await studentRepo.Exists(studentId))
            {
                //Update Details
                var updatedStudent = await studentRepo.UpdateStudent(studentId, mapper.Map<Student>(request));

                if (updatedStudent != null)
                {
                    return Ok(mapper.Map<Student>(updatedStudent));
                }
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("[controller]/{studentId:guid}")]
        public async Task<IActionResult> DeleteStudentAsync([FromRoute] Guid studentId)
        {
            if (await studentRepo.Exists(studentId))
            {
                //Delete the student
                var studentToDelete = await studentRepo.DeleteStudent(studentId);

                return Ok(mapper.Map<StudentDTO>(studentToDelete));
            }

            return NotFound();
        }

        [HttpPost]
        [Route("[controller]/Add")]
        public async Task<IActionResult> AddStudentAsync([FromBody] AddStudentRequest request)
        {
            var student = await studentRepo.AddStudent(mapper.Map<Student>(request));
            return CreatedAtAction(nameof(GetStudentAsync), new { studentId = student.Id },
                mapper.Map<StudentDTO>(student));
        }
    }
}
