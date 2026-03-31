using System.Runtime.Serialization;

namespace HiveSync.Application.Contracts.Entities.Business;

/// <summary>
/// Represents the current state of a Todo item.
/// </summary>
public enum TodoState
{
    /// <summary>
    /// Todo has been created but not yet started.
    /// </summary>
    [EnumMember(Value = "ToDo")]
    ToDo = 1000,

    /// <summary>
    /// Todo is currently in progress.
    /// </summary>
    [EnumMember(Value = "InProgress")]
    InProgress = 2000,

    /// <summary>
    /// Todo has been completed.
    /// </summary>
    [EnumMember(Value = "Done")]
    Done = 3000
}
