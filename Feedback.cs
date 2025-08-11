namespace SWAD_assignment
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string Description { get; set; }
        public Student FromStudent { get; set; }
        public DateTime DateCreated { get; set; }

        public Feedback()
        {
            DateCreated = DateTime.Now;
        }
    }
}