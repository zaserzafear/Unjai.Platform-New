using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unjai.Platform.Application.Diagnostics;

public static class ActivitySourceExtensions
{
    public static Activity? StartMethodActivity(
        this ActivitySource source,
        Type type,
        ActivityKind kind = ActivityKind.Internal,
        [CallerMemberName] string member = "")
    {
        var fullName = $"{type.FullName}.{member}";

        var activity = source.StartActivity(fullName, kind);

        if (activity is not null)
        {
            activity.SetTag("code.function", member);
            activity.SetTag("code.namespace", type.FullName);
        }

        return activity;
    }
}
