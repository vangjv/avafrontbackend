using AutoGen.Core;
namespace AvaFront.AutoGen.Models
{
    public class ExtendedMessage : TextMessage
    {
        public ExtendedMessage(Role role, string content, string? from = null) : base(role, content, from)
        {
            DateCreated = DateTime.Now;
        }

        public string RoleValue { get;set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
