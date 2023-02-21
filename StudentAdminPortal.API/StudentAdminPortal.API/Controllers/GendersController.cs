using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repositories;

namespace StudentAdminPortal.API.Controllers
{
    [ApiController]
    public class GendersController : Controller
    {
        private readonly IStudentRepository studentRepo;
        private readonly IMapper mapper;
        public GendersController(IStudentRepository studentRepo, IMapper mapper) 
        {
            this.studentRepo = studentRepo;
            this.mapper = mapper; 
        }

        [HttpGet]
        [Route("[controller]")]
        public async Task<IActionResult> GetAllGenders()
        {
            var genderList = await studentRepo.GetGendersAsync();

            if (genderList == null || !genderList.Any()) return NotFound();

            return Ok(mapper.Map<List<GenderDTO>>(genderList));
        }
    }
}
