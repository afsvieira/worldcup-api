using WorldCup.Domain.Entities;

namespace WorldCup.Identity.Models;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Field(u => u.Id);
        descriptor.Field(u => u.Email);
        descriptor.Field(u => u.Name);
        descriptor.Field(u => u.PlanType);
        descriptor.Field(u => u.CreatedAt);
        descriptor.Field(u => u.IsActive);
        descriptor.Field(u => u.ApiKeys);
        descriptor.Ignore(u => u.EntraExternalId);
    }
}