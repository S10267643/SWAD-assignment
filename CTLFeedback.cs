namespace SWAD_assignment
{
    public class CTLFeedback
    {
        private List<User> _users;

        public CTLFeedback(List<User> users)
        {
            _users = users;
        }

        public List<Feedback> GetFeedbacksForStaff(int staffId)
        {
            var staff = GetStaffById(staffId);
            return staff?.ReceivedFeedback ?? new List<Feedback>();
        }

        public Feedback GetFeedbackById(int feedbackId, int staffId)
        {
            var staff = GetStaffById(staffId);
            return staff?.ReceivedFeedback.FirstOrDefault(f => f.FeedbackId == feedbackId);
        }

        public bool SubmitResponse(int staffId, int feedbackId, string response)
        {
            var staff = GetStaffById(staffId);
            if (staff == null || string.IsNullOrWhiteSpace(response))
                return false;

            staff.RespondToFeedback(feedbackId, response);
            return true;
        }

        public bool ReportFeedback(int staffId, int feedbackId, string reason, List<Report> reports)
        {
            var staff = GetStaffById(staffId);
            var feedback = staff?.ReceivedFeedback.FirstOrDefault(f => f.FeedbackId == feedbackId);

            if (feedback == null || string.IsNullOrWhiteSpace(reason))
                return false;

            staff.ReportFeedback(feedback, reason, reports);
            return true;
        }
        public bool DeleteFeedback(int feedbackId)
        {
            // Find the staff member who has this feedback
            var staffWithFeedback = _users.OfType<FoodStallStaff>()
                .FirstOrDefault(s => s.ReceivedFeedback.Any(f => f.FeedbackId == feedbackId));

            if (staffWithFeedback == null)
                return false;

            // Find the specific feedback to remove
            var feedbackToRemove = staffWithFeedback.ReceivedFeedback
                .FirstOrDefault(f => f.FeedbackId == feedbackId);

            if (feedbackToRemove == null)
                return false;

            staffWithFeedback.RemoveFeedback(feedbackToRemove);
            return true;
        }

        public List<FoodStallStaff> GetAllStaff()
        {
            return _users.OfType<FoodStallStaff>().ToList();
        }

        private FoodStallStaff GetStaffById(int staffId)
        {
            return _users.OfType<FoodStallStaff>().FirstOrDefault(s => s.StaffId == staffId);
        }
        public bool SubmitFeedback(int studentId, int staffId, string description)
        {
            var student = _users.OfType<Student>().FirstOrDefault(s => s.StudentId == studentId);
            var staff = _users.OfType<FoodStallStaff>().FirstOrDefault(s => s.StaffId == staffId);

            if (student == null || staff == null || string.IsNullOrWhiteSpace(description))
                return false;

            var feedback = new Feedback
            {
                FeedbackId = Program.GetNextFeedbackId(),
                Description = description,
                FromStudent = student,
                DateCreated = DateTime.Now
            };

            staff.ReceiveFeedback(feedback);
            return true;
        }
    }
}