using System;
using System.Collections.Generic;
using System.IO;
using cal_over_work_cs.Entities;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace cal_over_work_cs
{
    class Program
    {
        private const string Yes = "Y";
        private const string WorkStartHours = "09:00";
        private const string WorkEndHours = "18:30";
        private static string _month = "";
        
        private static List<string> WorkHoursList()
        {
            string[] data = File.ReadAllLines($"./input/workTime_{_month}.txt");
            List<string> workHours = new List<string>();
            foreach (string s in data)
                workHours.Add(s);
            return workHours;
        }

        private static void OutputToConsoleAndwWriteToFile(string message, List<string> dataList)
        {
            Console.WriteLine(message);
            dataList.Add(message);
        }

        private static void ExportToExcel(List<string> dataList, int totalOverWorkedHours)
        {
            if(Directory.Exists($"./output/{_month}/")) Directory.CreateDirectory($"./output/{_month}/");
            string path = $"./output/{_month}/workTime_output_{_month}.xlsx";

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet worksheet = workbook.CreateSheet($"workTime_{_month}");

                int rowIndex = 0;
                IRow row = null;
                foreach (string data in dataList)
                {
                    row = worksheet.CreateRow(rowIndex);
                    int columnIndex = 0;
                    string[] d = data.Split(", ");
                    foreach (string subData in d)
                    {
                        row.CreateCell(columnIndex).SetCellValue(subData);
                        columnIndex++;
                    }
                    rowIndex++;
                }

                row = worksheet.CreateRow(rowIndex);
                row.CreateCell(3).SetCellValue(totalOverWorkedHours);
                workbook.Write(fs);
            }
            Console.WriteLine("Done exporting to excel");
        }

        private static void CalculateWorkTime()
        {
            Console.WriteLine("Enter month for input file (2 digits): ");
            _month = Console.ReadLine();
            List<string> workHours = WorkHoursList();
            List<string> outputDataList = new List<string>();
            List<string> dataListForExcel = new List<string>();
            int totalOverWorkHours = 0;
            foreach (string s in workHours)
            {
                string[] data = s.Split(", ");
                var workTimeObject = new WorkTimeObject(data[0], data[1], data[2]);
                if (data.Length > 3)
                {
                    workTimeObject.IsNormalWorkDay = Yes.Equals(data[3]);
                }

                string dataStringForExcel = $"{workTimeObject.Date}, {workTimeObject.ArriveTime}, {workTimeObject.LeaveTime}";
                DateTime startTime = DateTime.ParseExact($"{workTimeObject.Month}-{workTimeObject.Day} {WorkStartHours}", "MM-dd HH:mm", null);
                DateTime endTime = DateTime.ParseExact($"{workTimeObject.Month}-{workTimeObject.Day} {WorkEndHours}", "MM-dd HH:mm", null);
                DateTime arriveTime = DateTime.ParseExact($"{workTimeObject.Month}-{workTimeObject.Day} {workTimeObject.ArriveTime}", "MM-dd HH:mm", null);
                DateTime leaveTime = DateTime.ParseExact($"{workTimeObject.Month}-{workTimeObject.Day} {workTimeObject.LeaveTime}", "MM-dd HH:mm", null);
                
                OutputToConsoleAndwWriteToFile($"date: {workTimeObject.Date}", outputDataList);
                OutputToConsoleAndwWriteToFile($"actual arrive time: {arriveTime:yyyy-MM-dd HH:mm}", outputDataList);
                OutputToConsoleAndwWriteToFile($"actual leave time: {leaveTime:yyyy-MM-dd HH:mm}", outputDataList);
                if (arriveTime.DayOfWeek == DayOfWeek.Saturday && !workTimeObject.IsNormalWorkDay)
                {
                    // weekend calculation
                    OutputToConsoleAndwWriteToFile("isSaturday", outputDataList);
                    TimeSpan timeDiff = leaveTime - arriveTime;
                    OutputToConsoleAndwWriteToFile($"overworked in minutes: {timeDiff.TotalMinutes}", outputDataList);
                    OutputToConsoleAndwWriteToFile($"overworked in hours: {Math.Floor(timeDiff.TotalHours)}", outputDataList);
                    totalOverWorkHours += (int) Math.Floor(timeDiff.TotalHours);
                    OutputToConsoleAndwWriteToFile("\n", outputDataList);
                    string timeDiffStr = Math.Floor(timeDiff.TotalHours) > 0 ? $"+{Math.Floor(timeDiff.TotalHours)}" : $"{Math.Floor(timeDiff.TotalHours)}";
                    dataListForExcel.Add($"{dataStringForExcel}, {timeDiffStr}");
                }
                else
                {
                    // normal workday calculation
                    TimeSpan differenceArriveTime = arriveTime - startTime;
                    if (arriveTime.Hour < 9)
                    {
                        differenceArriveTime = TimeSpan.Zero;
                    }
                    DateTime actualLeaveTime = endTime.Add(differenceArriveTime);
                    TimeSpan overWorked = leaveTime - actualLeaveTime;
                    
                    OutputToConsoleAndwWriteToFile($"leave time: {actualLeaveTime:yyyy-MM-dd HH:mm}", outputDataList);
                    OutputToConsoleAndwWriteToFile($"overworked in minutes: {overWorked.TotalMinutes}", outputDataList);
                    OutputToConsoleAndwWriteToFile($"overwork in hours: {Math.Floor(overWorked.TotalHours)}", outputDataList);
                    totalOverWorkHours += (int) Math.Floor(overWorked.TotalHours);
                    OutputToConsoleAndwWriteToFile("\n", outputDataList);
                    string timeDiffStr = Math.Floor(overWorked.TotalHours) > 0 ? $"+{Math.Floor(overWorked.TotalHours)}" : $"{Math.Floor(overWorked.TotalHours)}";
                    dataListForExcel.Add($"{dataStringForExcel}, {timeDiffStr}");
                }
            }
            OutputToConsoleAndwWriteToFile($"total over worked hours: {totalOverWorkHours}", outputDataList);
            
            // write to file
            if(!Directory.Exists($"./output/{_month}/")) Directory.CreateDirectory($"./output/{_month}/");
            File.WriteAllLines($"./output/{_month}/workTime_output_{_month}.txt", outputDataList.ToArray());
            
            // output to Excel
            ExportToExcel(dataListForExcel, totalOverWorkHours);
            
            Console.WriteLine("Done exporting to txt and xlsx, press any key to exit...");
            Console.ReadKey();
        }
        
        public static void Main(string[] args)
        {
            CalculateWorkTime();
        }
    }
}