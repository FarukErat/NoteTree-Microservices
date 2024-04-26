using Features.Authentication.Login;
using Features.Authentication.Register;
using Features.NoteTree.GetNotes;
using Features.NoteTree.SetNotes;

namespace Features;

public static class DependencyInjection
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services.AddSingleton<RegisterService>();
        services.AddSingleton<LoginService>();
        services.AddSingleton<GetNotesService>();
        services.AddSingleton<SetNotesService>();
        return services;
    }
}
