using System;

namespace SWAD_assignment
{
    public class Report
    {
        public int ReportId { get; set; }
        public int FeedbackId { get; set; }
        public int ReportedByStaffId { get; set; }
        public string Reason { get; set; }
        public DateTime DateReported { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public Feedback Feedback { get; set; }

        public Report(int reportId, int feedbackId, int reportedByStaffId, string reason, Feedback feedback)
        {
            ReportId = reportId;
            FeedbackId = feedbackId;
            ReportedByStaffId = reportedByStaffId;
            Reason = reason;
            DateReported = DateTime.Now;
            Status = "Pending";
            Feedback = feedback;
        }

        public void Approve()
        {
            Status = "Approved";
        }

        public void Reject()
        {
            Status = "Rejected";
        }
    }
}