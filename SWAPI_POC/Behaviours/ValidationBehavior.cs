using FluentValidation;
using MediatR;

namespace SWAPI_POC.Behaviours
{
    /// <summary>
    /// Represents the MediatR pipeline behavior to run validation logic before the handlers handle the request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="validators">The validators used for validating the request.</param>
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Handles the request by validating it before passing it to the next handler in the pipeline.
        /// </summary>
        /// <param name="request">The request to be validated.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="next">The next handler in the pipeline.</param>
        /// <returns>
        /// A task that represents the asynchronous operation and contains the response of the request.
        /// </returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);

                FluentValidation.Results.ValidationResult[] validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                List<FluentValidation.Results.ValidationFailure> failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                    throw new ValidationException(failures); ;
            }
            return await next();
        }


    }
}

