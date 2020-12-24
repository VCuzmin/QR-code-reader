using FluentValidation;
using MediatR.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QRCodeReader.Application.MediatorPipeline
{
    public class ValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var context = new ValidationContext(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return Task.CompletedTask;
        }
    }
}