using FluentValidation;
using Eme_Search.Modules.Search.DTOs;

namespace Eme_Search.Modules.Search.DTOs.Validators;

public class StandardSearchRequestValidator : AbstractValidator<SearchRequestDto>
{
    public StandardSearchRequestValidator()
    {
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(250);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue)
            .WithMessage("Longitude must be between -180 and 180.");
        
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                var hasLat = request.Latitude.HasValue;
                var hasLon = request.Longitude.HasValue;

                if (hasLat != hasLon)
                {
                    context.AddFailure("Latitude and Longitude must both be provided or both be null.");
                }
            });
        
        RuleFor(x => x.Radius)
            .InclusiveBetween(0, 40000)
            .When(x => x.Radius.HasValue)
            .WithMessage("Radius must be between 0 and 40000.");

        RuleForEach(x => x.Price)
            .InclusiveBetween(1, 4)
            .WithMessage("Price must be between 1 and 4.");

        RuleFor(x => x.ReservationCovers)
            .InclusiveBetween(1, 10)
            .WithMessage("ReservationCovers must be between 1 and 10.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(0, 50)
            .WithMessage("Limit must be between 0 and 50.");

        RuleFor(x => x.Offset)
            .InclusiveBetween(0, 1000)
            .WithMessage("Offset must be between 0 and 1000.");

        RuleFor(x => x.SortBy)
            .Must(val => new[] { "best_match", "rating", "review_count", "distance" }.Contains(val.ToLower()))
            .WithMessage("SortBy must be one of: best_match, rating, review_count, distance.");
    }
}