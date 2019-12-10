using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GradesNotification.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace GradesNotification.Services
{
    public class RitmService
    {
        ILogger<RitmService> _logger;

        public RitmService(ILogger<RitmService> logger)
        {
            _logger = logger;
        }

        private async Task<HtmlDocument> getHtmlDoc(Student student, string url)
        {
            try
            {
                var postData = new Dictionary<string, string>();
                postData["LoginForm[username]"] = student.RitmLogin;
                postData["LoginForm[password]"] = student.Password;
                postData["LoginForm[rememberMe]"] = "0";
                postData["yt0"] = "";

                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                {
                    using (var httpClient = new HttpClient(handler))
                    {
                        using (var content = new FormUrlEncodedContent(postData))
                        {
                            content.Headers.Clear();
                            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                            var loginResponse = await httpClient.PostAsync("http://ritm.ispu.ru/login", content);
                            _logger.LogInformation($"Request completed, status = {loginResponse.StatusCode}");
                        }
                    }
                }

                var htmlWeb = new HtmlWeb();
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.CookieContainer = cookieContainer;

                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                var streamreader = new StreamReader(stream);
                var str = await streamreader.ReadToEndAsync();

                var document = new HtmlDocument();
                document.LoadHtml(str);
                return document;
            } catch (Exception e)
            {
                var message = $"Couldn't connect to the Ritm for student: {student.RitmLogin}. Exception: {e.ToString()}";
                _logger.LogError(message);
                throw new Exception(message, e);
            }
        }

        public async Task<bool> CheckStudentPassword(Student student)
        {
            try
            {
                var doc = await getHtmlDoc(student, "http://ritm.ispu.ru/profile/grades");
                var node = doc.DocumentNode.SelectSingleNode("//title");
                return node.InnerText != "Вход в систему / РИТМ.Рейтинг";
            } 
            catch(Exception e)
            {
                _logger.LogError($"Couldn't check user {student.RitmLogin}. Exception: {e}");
                return false;
            }
        }

        public async Task<List<Semester>> ParseAllSemesters(Student student)
        {
            var semestrList = new List<Semester>();
            for (int i = 1; i <= 12; i++)
            {
                try
                {
                    var subjects = await parseSemester(student, i);
                    if (subjects.Count() == 0) 
                    {
                        break;
                    }

                    semestrList.Add(new Semester() { Number = i, Subjects = subjects});
                }  
                catch (Exception e)
                {
                    break;
                }
            }

            return semestrList;
        }

        private async Task<List<Subject>> parseSemester(Student student, int semester)
        {
            try
            {
                var doc = await getHtmlDoc(student, $"http://ritm.ispu.ru/profile/grades?semester={semester}");
                var nodes = doc.DocumentNode.SelectNodes("//td");


                var list = new List<Subject>();
                if (nodes.Count() % 8 != 0)
                {
                    throw new Exception("Couldn't parse grades table");
                }

                for (int i = 0; i < nodes.Count(); i += 8)
                {
                    var subject = new Subject()
                    {
                        Semestr = semester,
                        Name = nodes[i + 0].InnerText,
                        Test1 = nodes[i + 1].InnerText,
                        Test2 = nodes[i + 2].InnerText,
                        Test3 = nodes[i + 3].InnerText,
                        Test4 = nodes[i + 4].InnerText,
                        Rating = nodes[i + 5].InnerText,
                        Exam = nodes[i + 6].InnerText,
                        Grade = nodes[i + 7].InnerText,
                    };

                    list.Add(subject);
                }

                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Couldn't check user {student.RitmLogin}, semester {semester}. Exception: {e.ToString()}");
                return new List<Subject>();
            }
        }
    }
}
