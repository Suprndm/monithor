using Monithor.Definitions;

namespace Monithor.Dtos
{
    public class Error
    {
        public Error(string name, string description, ErrorCode code)
        {
            Name = name;
            Description = description;
            Code = code;
        }

        public string Name { get;}
        public string Description { get; }
        public ErrorCode Code { get; }
    }
}
