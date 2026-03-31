using HiveSync.Application.Contracts.Entities.Business;
using HiveSync.Application.Contracts.Persistence;
using HiveSync.Application.Contracts.Projects.Queries.Dto;
using HiveSync.Application.Contracts.Sections.Queries;
using HiveSync.Application.Contracts.Sections.Queries.Dto;

namespace HiveSync.Application.Handlers.Sections.Queries;

/// <summary>
/// Handles retrieval of a single <see cref="Section"/> entity
/// projected to <see cref="SectionDetailDto"/>.
/// </summary>
/// <param name="sectionRepository">Read-only repository used to access section data.</param>
/// <param name="projectRepository">Read-only repository used to access project data</param>
public class GetSectionDtoQueryHandler(
    IReadOnlyRepository<Section> sectionRepository,
    IReadOnlyRepository<Project> projectRepository)
    : IRequestHandler<GetSectionDtoQuery, SectionDetailDto>
{
    public async Task<SectionDetailDto> Handle(
        GetSectionDtoQuery request, CancellationToken cancellationToken)
    {
        // Fetch the section
        var section = await sectionRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Section {request.Id} not found");

        // Fetch the parent project
        var project = await projectRepository.GetByIdAsync(section.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Project not found for Section ID {section.Id}");

        // Map to DTO including Project
        var dto = new SectionDetailDto
        {
            Id = section.Id,
            Name = section.Name,
            Color = section.Color,
            Project = new ProjectDetailDto
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color
            }
        };

        return dto;
    }
}
