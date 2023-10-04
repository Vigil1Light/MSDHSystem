using System;
using System.Collections.Generic;
using System.Text;

namespace MSDHSystem.Models
{
    public class TimeStudyDate
    {
        public string month { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string status { get; set; }
        public bool IsEnabledCell { get; set; }
    }
}
