using System;

namespace cal_over_work_cs.Entities
{
    public class WorkTimeObject
    {
        public WorkTimeObject(string date, string arriveTime, string leaveTime)
        {
            this.Date = date;
            this.ArriveTime = arriveTime;
            this.LeaveTime = leaveTime;
        }

        public string Date { get; set; }
        public string ArriveTime { get; set; }
        public string LeaveTime { get; set; }
        public bool IsNormalWorkDay { get; set; }

        public string Month => Date[..2];
        public string Day => Date[2..];
    }
}