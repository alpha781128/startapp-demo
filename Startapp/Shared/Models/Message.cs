using System;
using System.ComponentModel.DataAnnotations;

namespace Startapp.Shared.Models
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        public string MessageBody { get; set; }
        public DateTime Created { get; set; }
        public Boolean Viewed { get; set; }
        public Conversation Conversation { get; set; }
        public string UserId { get; set; }
    }
}
