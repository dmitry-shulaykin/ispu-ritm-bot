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
        private readonly RitmService _ritmService;
        public StudentsRepository(IOptions<MongoOptions> options, RitmService ritmService)
        {
            _ritmService = ritmService;
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _students = database.GetCollection<Student>(options.Value.StudentsCollectionName);
        }

        public async Task<Student> Create(Student student)
        {
            if (await _ritmService.CheckStudentPassword(student))
            {
                student.Semesters = await _ritmService.ParseAllSemesters(student);
                _students.InsertOne(student);
                return student;
            } 
            else
            {
                throw new Exception("Couldn't check student credentials");
            }
        }

        public List<Student> GetAll() => _students.Find(student => true).ToList();
        public Student GetById(string id) => _students.Find(student => student.Id == id).FirstOrDefault();
        public Student GetByRitmLogin(string ritmLogin) => _students.Find(student => student.RitmLogin == ritmLogin).FirstOrDefault();
        public void Update(string id, Student student) => _students.ReplaceOne(student => student.Id == id, student);
        public void Remove(Student student) => _students.DeleteOne(student => student.Id == student.Id);
        public void Remove(string id) => _students.DeleteOne(student => student.Id == id);
    }
}
