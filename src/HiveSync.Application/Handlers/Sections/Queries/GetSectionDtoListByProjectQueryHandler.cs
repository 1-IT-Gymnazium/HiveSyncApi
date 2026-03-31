using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using HiveSync.Application.Contracts.Sections.Queries;
using HiveSync.Application.Contracts.Sections.Queries.Dto;

namespace HiveSync.Application.Handlers.Sections.Queries;

/// <summary>
/// Handles retrieval of a list of <see cref="Section"/> entities projected to <see cref="SectionDetailDto"/> for a specific project.
/// </summary>
/// <param name="sectionRepository">Read-only repository used to access section data.</param>
/// <param name="projectRepository">Read-only repository used to access project data</param>
public class GetSectionDtoListByProjectQueryHandler(
     IReadOnlyRepository<Section> sectionRepository,
     IReadOnlyRepository<Project> projectRepository)
     : IRequestHandler<GetSectionDtoListByProjectQuery, IEnumerable<SectionDetailDto?>>
{
    public async Task<IEnumerable<SectionDetailDto?>> Handle(
        GetSectionDtoListByProjectQuery request, CancellationToken cancellationToken)
    {
        // Fetch all sections for this project
        var sections = (await sectionRepository.ListAsync(cancellationToken))
            .Where(s => s.ProjectId == request.ProjectId)
            .OrderBy(s => s.Order)
            .ToList();

        if (sections.Count == 0)
            return [];

        // Fetch the project (single project for all sections)
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Project {request.ProjectId} not found");

        // Map sections to DTOs including Project
        var result = sections.Select(s => new SectionDetailDto
        {
            Id = s.Id,
            Name = s.Name,
            Color = s.Color,
            Project = new ProjectDetailDto
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color
            }
        });

        return result;
    }
}
