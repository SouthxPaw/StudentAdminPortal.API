using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.API.DataModels;
using StudentAdminPortal.API.DomainModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentAdminPortal.API.Repositories
{
    public class SqlStudentRepository : IStudentRepository
    {
        private readonly StudentAdminContext context;
        public SqlStudentRepository(StudentAdminContext context)
        {
            this.context = context;
        }

        public async Task<List<Student>> GetStudentsAsync()
        {
            return await context.Student.Include(nameof(Gender)).Include(nameof(Address)).ToListAsync();
        }

        public async Task<Student> GetStudentAsync(Guid studentId)
        {
            return await context.Student.Include(nameof(Gender)).Include(nameof(Address))
                .FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task<List<Gender>> GetGendersAsync()
        {
            return await context.Gender.ToListAsync();
        }

        public async Task<bool> Exists(Guid studentId)
        {
            return await context.Student.AnyAsync(x => x.Id == studentId);
        }

        public async Task<Student> UpdateStudent (Guid studentId, Student studentToUpdate)
        {
            var existingStudent = await GetStudentAsync(studentId);

            if (existingStudent != null)
            {
                existingStudent.FirstName = studentToUpdate.FirstName;
                existingStudent.LastName = studentToUpdate.LastName;
                existingStudent.DateOfBirth = studentToUpdate.DateOfBirth;
                existingStudent.Email = studentToUpdate.Email;
                existingStudent.Mobile = studentToUpdate.Mobile;
                existingStudent.GenderId = studentToUpdate.GenderId;
                existingStudent.Address.PhysicalAddress = studentToUpdate.Address.PhysicalAddress;
                existingStudent.Address.PostalAddress = studentToUpdate.Address.PostalAddress;

                await context.SaveChangesAsync();
                return existingStudent;
            }

            return null;
        }

        public async Task<Student> DeleteStudent (Guid studentId)
        {
            var studentToDelete = await GetStudentAsync(studentId);

            if (studentToDelete != null) 
            {
                context.Student.Remove(studentToDelete);
                await context.SaveChangesAsync();
                return studentToDelete;
            }

            return null;
        }

    }
}
