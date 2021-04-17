using System.ComponentModel.DataAnnotations;

namespace Startapp.Shared.Models
{
    public class UsersConversation
    {
        public int Id { get; set; }
        [Required]
        public Conversation Conversation { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
