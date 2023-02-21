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
        private readonly IImageRepository imageRepo;
        public StudentsController(IStudentRepository studentRepo, IMapper mapper, IImageRepository imageRepo)
        {
            this.studentRepo = studentRepo;
            this.mapper = mapper;
            this.imageRepo = imageRepo;
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

        [HttpPost]
        [Route("[controller]/{studentId:guid}/upload-image")]
        public async Task<IActionResult> UploadProfileImage([FromRoute] Guid studentId, IFormFile profileImage)
        {
            var validExtensions = new List<string> 
            {
                ".jpeg",
                ".png",
                ".gif",
                ".jpg"
            };

            if (profileImage != null && profileImage.Length > 0)
            {
                var extension = Path.GetExtension(profileImage.FileName);
                if(validExtensions.Contains(extension))
                {
                    // Search if student id exists in dB
                    if (await studentRepo.Exists(studentId))
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);
                        // Upload image to local storage
                        var fileImagePath = await imageRepo.Upload(profileImage, fileName);

                        // Update profile image path db
                        if (await studentRepo.UpdateProfileImage(studentId, fileImagePath))
                        {
                            return Ok(fileImagePath);
                        }

                        return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading image");
                    }
                }

                return BadRequest("This is not a valid image format");
            }

            return NotFound();
        }
    }
}
