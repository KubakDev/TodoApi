using System.Text.Json.Serialization;

namespace TodoApi.Models.Common
{
  public sealed class BasicResponse
  {
    /// <summary>
    /// Error code used to define the type of error
    /// </summary>
    [JsonPropertyName("code")]
    public ErrorCode Code { get; }

    /// <summary>
    /// A short message explaining what went wrong
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; }

    public BasicResponse(in string message)
    {
      Code = ErrorCode.Unknown;
      Message = message;
    }

    public BasicResponse(in ErrorCode code)
    {
      Code = code;
      Message = code.ToString();
    }

    public BasicResponse(in ErrorCode code, in string message)
    {
      Code = code;
      Message = message;
    }

    /// <summary>
    /// Represents an unknown error.
    /// </summary>
    public static readonly BasicResponse Unknown = new(ErrorCode.Unknown, "An unknown error occured.");

    /// <summary>
    /// Represents no error, used for successful operations.
    /// </summary>
    /// <returns></returns>
    public static readonly BasicResponse Successful =
        new(ErrorCode.Success, "Request handled successfully.");



    /// <summary>
    /// Represents error when resource creation fails.
    /// </summary>
    public static readonly BasicResponse CouldNotCreateResource =
        new(ErrorCode.CouldNotCreateResource, "Could not create the requested resource.");

    /// <summary>
    /// Represents error when updating a resource fails.
    /// </summary>
    public static readonly BasicResponse CouldNotUpdateResource =
        new(ErrorCode.CouldNotCreateResource, "Could not update the requested resource.");

    /// <summary>
    /// Represents error when deleting a resource fails.
    /// </summary>
    public static readonly BasicResponse CouldNotDeleteResource =
        new(ErrorCode.CouldNotDeleteResource, "Could not delete the requested resource.");

    /// <summary>
    /// Represents error when resource creation fails.
    /// </summary>
    public static readonly BasicResponse AuthenticationFailed =
        new(ErrorCode.AuthenticationFailed, "Authentication failed.");

    /// <summary>
    /// Represents error when reverting the status of an order fails.
    /// </summary>

    /// <summary>
    /// Represents no error, used for successful operations.
    /// </summary>
    public static BasicResponse Success(in string message)
    {
      return new(ErrorCode.Success, message);
    }

    /// <summary>
    /// Represents error when the requested resource does not exist
    /// </summary>
    /// <param name="resourceId">Id of the requested resource</param>
    public static BasicResponse ResourceDoesNotExist(in string resourceName,
                                                     in string resourceId)
    {
      return new(ErrorCode.ResourceDoesNotExist,
                                $"Resource '{resourceName}' with id '{resourceId}' does not exist.");
    }
  }
}