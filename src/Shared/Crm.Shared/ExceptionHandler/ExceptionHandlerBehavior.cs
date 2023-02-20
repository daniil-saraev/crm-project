using Ardalis.GuardClauses;
using Ardalis.Result;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Crm.Shared.ExceptionHandler
{
    public class ExceptionHandlerBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : IRequest<Result<TResponse>>
    {
        public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (NotFoundException ex)
            {
                return Result.NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = ex.Message } });
            }
            catch (ValidationException ex)
            {
                return Result.Invalid(new List<ValidationError> { new ValidationError { ErrorMessage = ex.Message } });
            }
            catch (Exception ex)
            {
                return Result.Error(ex.Message);
            }
        }
    }
}
