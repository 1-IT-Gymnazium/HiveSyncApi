using System.Runtime.Serialization;

namespace HiveSync.Application.Contracts.Entities.Business;

/// <summary>
/// Represents the priority level of a Todo item.
/// </summary>
public enum TodoPriority
{
    /// <summary>
    /// Low priority; can be done later.
    /// </summary>
    [EnumMember(Value = "Low")]
    Low = 1000,

    /// <summary>
    /// Medium priority; should be done in normal course.
    /// </summary>
    [EnumMember(Value = "Medium")]
    Medium = 2000,

    /// <summary>
    /// High priority; requires immediate attention.
    /// </summary>
    [EnumMember(Value = "High")]
    High = 3000
}
