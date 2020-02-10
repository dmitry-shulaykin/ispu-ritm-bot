using GradesNotification.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification.Services
{
    public class StudentsRepository
    {
        private readonly IMongoCollection<Student> _students;
        public StudentsRepository(IOptions<MongoOptions> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _students = database.GetCollection<Student>(options.Value.StudentsCollectionName);
        }

        public async Task<List<Student>> GetAll()
        {
            var students = await _students.FindAsync(student => true);
            return students.ToList();
        }

        public async Task<Student> GetByRitmLogin(string ritmLogin)
        {
            var students = await _students.FindAsync(student => student.RitmLogin == ritmLogin);
            return students.FirstOrDefault();
        }

        public async Task<List<Semester>> GetStudentSemesters(string ritmLogin)
        {
            var student = await GetByRitmLogin(ritmLogin);
            return student.Semesters;
        }

        public async Task<Semester> GetStudentSemester(string ritmLogin, int semesterNumber) 
        {
            var student = await GetByRitmLogin(ritmLogin);
            return student?.Semesters.FirstOrDefault(s => s.Number == semesterNumber);
        }

        public async Task<List<Subject>> GetStudentSubjects(string ritmLogin, int semesterNumber)
        {
            var semester = await GetStudentSemester(ritmLogin, semesterNumber);
            return semester?.Subjects;
        }

        public async Task<Subject> GetStudentSubject(string ritmLogin, int semesterNumber, string subjectName)
        {
            var semester = await GetStudentSemester(ritmLogin, semesterNumber);
            return semester?.Subjects.FirstOrDefault(s => s.Name == subjectName);
        }

        public async Task<Student> CreateStudent(Student student)
        {
            await _students.InsertOneAsync(student);
            return await GetByRitmLogin(student.RitmLogin);
        }

        public async Task<Student> CreateSemester(string ritmLogin, Semester semester)
        {
            var student = await GetByRitmLogin(ritmLogin);

            if (student.Semesters.FirstOrDefault(s => s.Number == semester.Number) != null)
            {
                throw new Exception("Semester already exists");
            }

            student.Semesters.Add(semester);

            return await UpdateStudent(student);
        }

        public async Task<Student> CreateSubject(string ritmLogin, int semesterNumber, Subject subject)
        {
            var student = await GetByRitmLogin(ritmLogin);
            var semester = student.Semesters.Find(s => s.Number == semesterNumber);

            if (semester.Subjects.FirstOrDefault(s => s.Name == subject.Name) != null)
            {
                throw new Exception("Subject already exists");
            }

            semester.Subjects.Add(subject);
            return await UpdateStudent(student);
        }

        public async Task<Student> UpdateStudent (Student student)
        {
            await _students.ReplaceOneAsync(s => s.RitmLogin == student.RitmLogin, student);
            return await GetByRitmLogin(student.RitmLogin);
        }
        
        public async Task<Semester> UpdateSemester (string ritmLogin, Semester semester)
        {
            var student = await GetByRitmLogin(ritmLogin);
            var existignSemester = student.Semesters.Find(s => s.Number == semester.Number);
            student.Semesters.Remove(existignSemester);
            student.Semesters.Add(semester);
            await UpdateStudent(student);
            return await GetStudentSemester(ritmLogin, semester.Number);
        }

        public async Task<Subject> UpdateSubject (string ritmLogin, int semesterNumber, Subject subject)
        {
            var student = await GetByRitmLogin(ritmLogin);
            var semester = student.Semesters.Find(s => s.Number == semesterNumber);
            var existingSubject = semester.Subjects.Find(s => s.Name == subject.Name);
            semester.Subjects.Remove(existingSubject);
            semester.Subjects.Add(subject);
            await UpdateStudent(student);
            return await GetStudentSubject(ritmLogin, semester.Number, subject.Name);
        }

        public async Task DeleteStudent(string ritmLogin)
        {
            await _students.DeleteOneAsync(s => s.RitmLogin == ritmLogin);
        }

        public async Task DeleteSemester(string ritmLogin, int semesterNumber)
        {
            var student = await GetByRitmLogin(ritmLogin);
            var existignSemester = student.Semesters.Find(s => s.Number == semesterNumber);
            student.Semesters.Remove(existignSemester);
            await UpdateStudent(student);
        }

        public async Task DeleteSubject(string ritmLogin, int semesterNumber, string subjectName)
        {
            var student = await GetByRitmLogin(ritmLogin);
            var semester = student.Semesters.Find(s => s.Number == semesterNumber);
            var subject = semester.Subjects.Find(s => s.Name == subjectName);
            semester.Subjects.Remove(subject);
            await UpdateStudent(student);
        }

        public async Task<Student> GetByChatId(long chatId)
        {
            var student = await _students.FindAsync(student => student.ChatId == chatId);
            return student.FirstOrDefault();
        }
    }
}
