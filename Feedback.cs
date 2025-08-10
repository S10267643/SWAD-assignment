using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWAD_assignment
{
    public class Feedback
    {
        public string Description { get; set; }
        public int FeedbackId { get; set; }

        public Feedback()
        {
        }

        public Feedback(string description, int feedbackId)
        {
            Description = description;
            FeedbackId = feedbackId;
        }
    }
}
