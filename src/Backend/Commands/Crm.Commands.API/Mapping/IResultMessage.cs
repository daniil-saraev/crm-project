namespace Crm.Commands.API
{
    public interface IResultMessage
    {
        public Ok Ok { get; set; }
        public NotFound NotFound { get; set; }
        public Invalid Invalid { get; set; }
        public Error Error { get; set; }
    }
}