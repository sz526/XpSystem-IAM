using XpSystem;

namespace XpSystem.Api.Auth;

public class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public HttpCurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

    public string Id =>
        _accessor.HttpContext?.User.FindFirst("sub")?.Value
        ?? throw new InvalidOperationException("Ingen innlogget bruker");
}
