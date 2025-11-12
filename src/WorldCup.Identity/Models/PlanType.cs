using WorldCup.Domain.Entities;

namespace WorldCup.Identity.Models;

public class PlanType : ObjectType<Plan>
{
    protected override void Configure(IObjectTypeDescriptor<Plan> descriptor)
    {
        descriptor.Field(p => p.Type);
        descriptor.Field(p => p.Name);
        descriptor.Field(p => p.Price);
        descriptor.Field(p => p.DailyRequestLimit);
        descriptor.Field(p => p.MinuteRequestLimit);
        descriptor.Field(p => p.MaxApiKeys);
        descriptor.Field(p => p.HasRestAccess);
        descriptor.Field(p => p.HasGraphQLAccess);
    }
}