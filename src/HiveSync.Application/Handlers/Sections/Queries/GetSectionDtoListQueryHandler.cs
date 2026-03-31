using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using HiveSync.Application.Contracts.Sections.Queries;
using HiveSync.Application.Contracts.Sections.Queries.Dto;

namespace HiveSync.Application.Handlers.Sections.Queries;

/// <summary>
/// Handles retrieval of a list of <see cref="Section"/> entities projected to <see cref="SectionDetailDto"/>.
/// </summary>
/// <param name="sectionRepository">Read-only repository used to access section data.</param>
/// <param name="projectRepository">Read-only repository used to access project data</param>
public class GetSectionDtoListQueryHandler(
    IReadOnlyRepository<Section> sectionRepository,
    IReadOnlyRepository<Project> projectRepository)
    : IRequestHandler<GetSectionDtoListQuery, IEnumerable<SectionDetailDto>>
{
    public async Task<IEnumerable<SectionDetailDto>> Handle(
        GetSectionDtoListQuery request, CancellationToken cancellationToken)
    {
        // Fetch all sections
        var sections = (await sectionRepository.ListAsync(cancellationToken))
            .OrderBy(s => s.Order)
            .ToList();

        if (sections.Count == 0)
            return [];

        // Fetch all projects used by these sections
        var projectIds = sections.Select(s => s.ProjectId).Distinct();
        var projects = (await projectRepository.ListAsync(cancellationToken))
            .Where(p => projectIds.Contains(p.Id))
            .ToDictionary(p => p.Id);

        // Map sections to DTOs including Project
        var result = sections.Select(s =>
        {
            if (!projects.TryGetValue(s.ProjectId, out var projectEntity) || projectEntity == null)
            {
                throw new NotFoundException($"Project not found for Section ID {s.Id}");
            }

            return new SectionDetailDto
            {
                Id = s.Id,
                Name = s.Name,
                Color = s.Color,
                Project = new ProjectDetailDto
                {
                    Id = projectEntity.Id,
                    Name = projectEntity.Name,
                    Color = projectEntity.Color
                }
            };
        });

        return result;
    }
}
