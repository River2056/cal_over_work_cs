using System;
using System.Collections.Generic;
using System.IO;

namespace cal_over_work_cs
{
    class Program
    { 
        private const string workStartHours = "09:00";
        private const string workEndHours = "18:30";
        private static string MONTH = "";
        
        static List<string> WorkHoursList()
        {
            string[] data = File.ReadAllLines($"./input/workTime_{MONTH}.txt");
            List<string> workHours = new List<string>();
            foreach (string s in data)
                workHours.Add(s);
            return workHours;
        }

        static void OutputToConsoleAndwWriteToFile(string message, List<string> dataList)
        {
            Console.WriteLine(message);
            dataList.Add(message);
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Enter month for input file (2 digits): ");
            MONTH = Console.ReadLine();
            List<string> workHours = WorkHoursList();
            List<string> outputDataList = new List<string>();
            int totalOverWorkHours = 0;
            foreach (string s in workHours)
            {
                string[] data = s.Split(", ");
                string month = data[0].Substring(0, 2);
                string day = data[0].Substring(2);
                DateTime startTime = DateTime.ParseExact($"{month}-{day} {workStartHours}", "MM-dd HH:mm", null);
                DateTime endTime = DateTime.ParseExact($"{month}-{day} {workEndHours}", "MM-dd HH:mm", null);
                DateTime arriveTime = DateTime.ParseExact($"{month}-{day} {data[1]}", "MM-dd HH:mm", null);
                DateTime leaveTime = DateTime.ParseExact($"{month}-{day} {data[2]}", "MM-dd HH:mm", null);
                
                OutputToConsoleAndwWriteToFile($"date: {data[0]}", outputDataList);
                OutputToConsoleAndwWriteToFile($"actual arrive time: {arriveTime:yyyy-MM-dd HH:mm}", outputDataList);
                OutputToConsoleAndwWriteToFile($"actual leave time: {leaveTime:yyyy-MM-dd HH:mm}", outputDataList);
                if (arriveTime.DayOfWeek == DayOfWeek.Saturday)
                {
                    OutputToConsoleAndwWriteToFile("isSaturday", outputDataList);
                    TimeSpan timeDiff = leaveTime - arriveTime;
                    OutputToConsoleAndwWriteToFile($"overworked in minutes: {timeDiff.TotalMinutes}", outputDataList);
                    OutputToConsoleAndwWriteToFile($"overworked in hours: {Math.Floor(timeDiff.TotalHours)}", outputDataList);
                    totalOverWorkHours += (int) Math.Floor(timeDiff.TotalHours);
                    OutputToConsoleAndwWriteToFile("\n", outputDataList);
                }
                else
                {
                    TimeSpan differenceArriveTime = arriveTime - startTime;
                    DateTime actualLeaveTime = endTime.Add(differenceArriveTime);
                    TimeSpan overWorked = leaveTime - actualLeaveTime;
                    
                    OutputToConsoleAndwWriteToFile($"leave time: {actualLeaveTime:yyyy-MM-dd HH:mm}", outputDataList);
                    OutputToConsoleAndwWriteToFile($"overworked in minutes: {overWorked.TotalMinutes}", outputDataList);
                    OutputToConsoleAndwWriteToFile($"overwork in hours: {Math.Floor(overWorked.TotalHours)}", outputDataList);
                    totalOverWorkHours += (int) Math.Floor(overWorked.TotalHours);
                    OutputToConsoleAndwWriteToFile("\n", outputDataList);
                }
            }
            OutputToConsoleAndwWriteToFile($"total over worked hours: {totalOverWorkHours}", outputDataList);
            
            // write to file
            File.WriteAllLines($"./output/workTime_output_{MONTH}.txt", outputDataList.ToArray());
        }
    }
}