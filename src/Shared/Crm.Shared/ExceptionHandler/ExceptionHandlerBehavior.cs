using Ardalis.GuardClauses;
using Ardalis.Result;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Crm.Shared.ExceptionHandler
{
    public class ExceptionHandlerBehavior<TCommand>
    {
        public async Task<IResult> Handle(TCommand command, RequestHandlerDelegate<IResult> next, CancellationToken cancellationToken)
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
