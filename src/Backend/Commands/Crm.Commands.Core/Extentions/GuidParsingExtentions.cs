namespace Crm.Commands.Core.Extentions
{
    public static class GuidParsingExtentions
    {
        public static Guid ToGuid(this string id)
        {
            return Guid.Parse(id);
        }
    }
}
