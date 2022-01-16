using System.Security.Claims;

namespace TodoApi.Extensions;

public static class ClaimsPrincipalExtensions
{
  public static string GetId(this ClaimsPrincipal user)
      => user.FindFirstValue(ClaimTypes.NameIdentifier);

  public static bool HasId(this ClaimsPrincipal user, in string id)
      => user.GetId() == id;
}
