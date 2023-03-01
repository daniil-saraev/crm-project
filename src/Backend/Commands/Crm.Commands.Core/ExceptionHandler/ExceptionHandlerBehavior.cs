using Ardalis.GuardClauses;
using Ardalis.Result;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Crm.Commands.Core.ExceptionHandler
{
    public class ExceptionHandlerBehaviorReturnResult<TRequest, TResult>
        : IPipelineBehavior<TRequest, TResult>
        where TRequest : IRequest<TResult>
        where TResult : Result
    {
        public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (NotFoundException ex)
            {
                return (TResult)Result.NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return (TResult)Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = ex.Message } });
            }
            catch (ValidationException ex)
            {
                return (TResult)Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = ex.Message } });
            }
            catch (FormatException ex)
            {
                return (TResult)Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = ex.Message } });
            }
            catch (Exception ex)
            {
                return (TResult)Result.Error(ex.Message);
            }
        }
    }
    
    public class ExceptionHandlerBehaviorReturnResultWithGuid<TRequest, TResult>
        : IPipelineBehavior<TRequest, TResult>
        where TRequest : IRequest<TResult>
        where TResult : Result<Guid>
    {
        public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (NotFoundException ex)
            {
                return (TResult)Result.NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return (TResult)Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = ex.Message } });
            }
            catch (ValidationException ex)
            {
                return (TResult)Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = ex.Message } });
            }
            catch (FormatException ex)
            {
                return (TResult)Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = ex.Message } });
            }
            catch (Exception ex)
            {
                return (TResult)Result.Error(ex.Message);
            }
        }
    }
}
