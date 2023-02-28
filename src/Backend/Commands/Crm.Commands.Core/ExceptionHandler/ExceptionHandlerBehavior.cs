using Ardalis.GuardClauses;
using Ardalis.Result;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Crm.Commands.Core.ExceptionHandler
{
    public class ExceptionHandlerBehavior<TRequest, TResult>
        : IPipelineBehavior<TRequest, Result<TResult>>
        where TRequest : IRequest<Result<TResult>>
    {
        public async Task<Result<TResult>> Handle(TRequest request, RequestHandlerDelegate<Result<TResult>> next, CancellationToken cancellationToken)
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

    public class ExceptionHandlerBehavior<TRequest>
        : IPipelineBehavior<TRequest, Result>
        where TRequest : IRequest<Result>
    {
        public async Task<Result> Handle(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken cancellationToken)
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
