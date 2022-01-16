namespace TodoApi.Models.Common
{
  public enum ErrorCode
  {
    Unknown = -1,
    Success = 0,

    // Common
    ResourceDoesNotExist,
    CouldNotCreateResource,
    CouldNotUpdateResource,
    CouldNotDeleteResource,

    // Authentication
    AuthenticationFailed = 1_101,

  }
}