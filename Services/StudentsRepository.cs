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
            _students.InsertOne(student);
            return student;
        }

        public List<Student> GetAll() => _students.Find(student => true).ToList();
        public Student GetById(string id) => _students.Find(student => student.Id == id).FirstOrDefault();
        public Student GetByRitmLogin(string ritmLogin) => _students.Find(student => student.RitmLogin == ritmLogin).FirstOrDefault();
        public Student GetByChatId(long chatId) => _students.Find(student => student.ChatId == chatId).FirstOrDefault();

        public void Update(string id, Student student) => _students.ReplaceOne(s => s.Id == id, student);

        public async Task<bool> UpdateOrRegisterStudentWithChatIdAsync (long chatId, string ritmLogin)
        {
            var student = GetByRitmLogin(ritmLogin);

            if (student == null)
            {
                student = new Student()
                {
                    RitmLogin = ritmLogin,
                    ChatId = chatId,
                };

                await Create(student);
                return true;
            } 
            else
            {
                student.ChatId = chatId;
                Update(student.Id, student);
                return false;
            }
        }

        public async Task UpdateStudentPasswordASync(long chatId, string ritmPassword) 
        {
            var student = GetByChatId(chatId);

            if (student == null) 
            {
                throw new Exception("Couldn't ritm login");
            }

            student.Password = ritmPassword;
            var check = await _ritmService.CheckStudentPassword(student);
            if (!check) 
            {
                throw new Exception($"Your credentials for {student.RitmLogin} don't work or ritm service unavailable");
            }

            try
            {
                student.Semesters = await _ritmService.ParseAllSemesters(student);
            } 
            catch
            {
                
            }

            Update(student.Id, student);
        }

        public void Remove(Student student) => _students.DeleteOne(s => s.Id == student.Id);
        public void Remove(string id) => _students.DeleteOne(student => student.Id == id);
    }
}
