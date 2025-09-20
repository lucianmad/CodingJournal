using System.Reflection;
using CodingJournal.Application.Features.Documents.Actions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CodingJournal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetDocumentsQuery).Assembly));
        services.AddValidatorsFromAssembly(typeof(GetDocumentsQuery).Assembly);
        return services;
    }
}