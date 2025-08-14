using SWAD_assignment;

public class Feedback
{
    public int FeedbackId { get; set; }
    public string Description { get; set; }
    public Student FromStudent { get; set; }
    public DateTime DateCreated { get; set; }
    public string Response { get; set; }
    public DateTime? ResponseDate { get; set; }
    public bool IsReported { get; set; }

    public Feedback()
    {
        DateCreated = DateTime.Now;
    }

    public bool HasResponse => !string.IsNullOrWhiteSpace(Response);
}