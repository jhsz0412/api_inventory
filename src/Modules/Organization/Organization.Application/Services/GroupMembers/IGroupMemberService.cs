
using Organization.Application.DTOs.GroupMembers;

namespace Organization.Application.Services.GroupMembers;

public interface IGroupMemberService
{
    Task<IReadOnlyList<GroupMemberResponse>> GetMembersAsync(
        Guid groupId,
        CancellationToken cancellationToken = default);

    Task<int> AddMembersAsync(
        Guid groupId,
        AddGroupMembersRequest request,
        CancellationToken cancellationToken = default);

    Task<int> CopyMembersAsync(
        Guid groupId,
        Guid sourceGroupId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveMemberAsync(
        Guid groupId,
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
