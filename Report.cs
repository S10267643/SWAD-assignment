using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Report
    {
        public int ReportId { get; set; }
        public string Reason { get; set; }
        public string ReportStatus { get; set; }

        public Report()
        {
        }

        public Report(int reportId, string reason, string reportStatus)
        {
            ReportId = reportId;
            Reason = reason;
            ReportStatus = reportStatus;
        }
    }
}
