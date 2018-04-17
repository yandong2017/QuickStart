using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StackExchange.Redis;
using System.Data.Entity;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace AspNetMvc.QuickStart.Models
{
    public class StudentDbContext
    {
        public string hashId = "Students";
        private ConnectionMultiplexer RedisConnection;
        public Dictionary<string, Student> DictStudents = new Dictionary<string, Student>();
        public IDatabase Students { get; set; }
        public StudentDbContext()
        {
            RedisConnection = ConnectionMultiplexer.Connect($"127.0.0.1:6379,password=1q2w3e");
            Students = RedisConnection.GetDatabase(9);
            DictStudents = new Dictionary<string, Student>();
            var allkey = Students.HashGetAll(hashId);
            foreach (var item in allkey)
            {
                Student student = DeserializeSingleData<Student>(item.Value);
                if (student != null)
                {
                    DictStudents[student.Name] = student;
                }
            }
        }

        public Student GetItemByName(string Name)
        {
            if (DictStudents.ContainsKey(Name))
            {
               return DictStudents[Name];
            }
            return null;
        }
        public void Save(Student student)
        {
            DictStudents[student.Name] = student;
            Students.HashSet($"{hashId}", $"{student.Name}", SerializerSingleData<Student>(student));
        }

        internal void Careate(Student student)
        {            
            DictStudents[student.Name] = student;
            Students.HashSet($"{hashId}", $"{student.Name}", SerializerSingleData<Student>(student));
        }

        public void Remove(Student student)
        {
            if (!DictStudents.ContainsKey(student.Name))
            {
                return;
            }
            DictStudents.Remove(student.Name);
            Students.HashDelete($"{hashId}", $"{student.Name}");
            throw new NotImplementedException();
        }
        public static string SerializerSingleData<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        /// <summary> 
        /// 反序列化单个对象
        /// </summary> 
        public static T DeserializeSingleData<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式  
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary> 
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串 
        /// </summary> 
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }


        /// <summary>  
        /// 将时间字符串转为Json时间 
        /// </summary> 
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
    }
}