using System;
using System.Collections.Generic;


namespace Startapp.Shared.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public Boolean Viewed { get; set; }
        public ICollection<UsersConversation> UsersConversations { get; set; }
        public string UserId { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
