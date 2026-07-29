using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Service.Interface;

namespace Service.Implementation;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid GetUserId()
    {
        return _accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}