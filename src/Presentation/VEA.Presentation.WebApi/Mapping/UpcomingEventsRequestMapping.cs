using VEA.Core.QueryContracts.Queries;
using VEA.Core.Tools.ObjectMapper;
using VEA.Presentation.WebApi.Endpoints.VeaEvents;

namespace VEA.Presentation.WebApi.Mapping;

public class UpcomingEventsRequestMapping : IMapping<GetUpcomingEventsRequest, UpcomingEventsPageQuery>
{
    public UpcomingEventsPageQuery Map(GetUpcomingEventsRequest input)
        => new(input.Page, input.PageSize, input.Title);
}