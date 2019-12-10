using GradesNotification.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradesNotification
{
    public class CrawlStudentJob : IJob
    {
        private readonly RitmService _ritmService;
        private readonly StudentsRepository _studentsRepository;
        private readonly string _ritmLogin;
        private readonly ILogger<CrawlStudentJob> _logger;

        public CrawlStudentJob(string ritmLogin, RitmService ritmService, StudentsRepository studentsRepository, ILogger<CrawlStudentJob> logger)
        {
            _ritmLogin = ritmLogin;
            _ritmService = ritmService;
            _studentsRepository = studentsRepository;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var student = _studentsRepository.GetByRitmLogin(_ritmLogin);
                var semesters = await _ritmService.ParseAllSemesters(student);

                var newSemestrs = new HashSet<int>();
                var newSubjects = new HashSet<string>();

                foreach (var semestr in semesters)
                {
                    foreach (var subject in semestr.Subjects)
                    {
                        var existsSemestr = student.Semesters.FirstOrDefault(s => s.Number == subject.Semestr);

                        if (existsSemestr == null)
                        {
                            newSemestrs.Add(subject.Semestr);
                            continue;
                        }

                        var existsSubject = existsSemestr.Subjects.FirstOrDefault(s => s.Name == subject.Name);

                        if (existsSemestr == null)
                        {
                            newSubjects.Add(subject.Name);
                            continue;
                        }
                    }
                }
            } 
            catch (Exception e)
            {
                _logger.LogError($"Error when crawling student {_ritmLogin}.  Exception: {e.ToString()}");
            }
        }
    }
}
