using Microsoft.AspNetCore.Http;

namespace HiveSync.Utilities.Error;

public class InternalServerException(string message)
    : HiveSyncException(
        StatusCodes.Status500InternalServerError,
        [
            new ApiErrorItem
            {
                Messages = [message]
            }
        ])
{
}
