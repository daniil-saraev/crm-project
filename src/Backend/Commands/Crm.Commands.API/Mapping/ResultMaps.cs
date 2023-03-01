using Ardalis.Result;

namespace Crm.Commands.API
{
    public partial class CompleteOrderResult : IResultMessage { }
    public partial class EditClientContactInfoResult : IResultMessage { }
    public partial class EditClientNameResult: IResultMessage { }
    public partial class EditOrderDescriptionResult : IResultMessage { }
    public partial class AddNewManagerResult: IResultMessage { }
    public partial class AssignClientResult : IResultMessage { }
    public partial class TransferClientResult : IResultMessage { }
    public partial class TransferManagerResult : IResultMessage { }

    public static class MapResult
    {
        public static TDestination MapTo<TDestination, K>(this Result<K> result) 
            where TDestination : IResultMessage, new() 
            where K : notnull
        {
            return result.Status switch
            {
                ResultStatus.Ok => new TDestination
                {
                    Ok = new Ok { Payload = result.Value.ToString() }
                },
                ResultStatus.Invalid => new TDestination
                {
                    Invalid = new Invalid { Message = result.ValidationErrors.FirstOrDefault()?.ErrorMessage }
                },
                ResultStatus.NotFound => new TDestination
                {
                    NotFound = new NotFound { Message = result.Errors.First() }
                },
                ResultStatus.Error => new TDestination
                {
                    Error = new Error { Message = result.Errors.First() }
                },
                _ => new TDestination
                {
                    Error = new Error { Message = result.Errors.First() }
                },
            };
        }

        public static TDestination MapTo<TDestination>(this Result result)
            where TDestination : IResultMessage, new()
        {
            return result.Status switch
            {
                ResultStatus.Ok => new TDestination
                {
                    Ok = new Ok { Payload = result.Value.ToString() }
                },
                ResultStatus.Invalid => new TDestination
                {
                    Invalid = new Invalid { Message = result.ValidationErrors.FirstOrDefault()?.ErrorMessage }
                },
                ResultStatus.NotFound => new TDestination
                {
                    NotFound = new NotFound { Message = result.Errors.First() }
                },
                ResultStatus.Error => new TDestination
                {
                    Error = new Error { Message = result.Errors.First() }
                },
                _ => new TDestination
                {
                    Error = new Error { Message = result.Errors.First() }
                },
            };
        }
    }
}
