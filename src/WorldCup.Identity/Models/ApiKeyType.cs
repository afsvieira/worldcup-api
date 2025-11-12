using WorldCup.Domain.Entities;

namespace WorldCup.Identity.Models;

public class ApiKeyType : ObjectType<ApiKey>
{
    protected override void Configure(IObjectTypeDescriptor<ApiKey> descriptor)
    {
        descriptor.Field(a => a.Id);
        descriptor.Field(a => a.Name);
        descriptor.Field(a => a.CreatedAt);
        descriptor.Field(a => a.ExpiresAt);
        descriptor.Field(a => a.IsActive);
        descriptor.Ignore(a => a.Key);
        descriptor.Ignore(a => a.UserId);
        descriptor.Ignore(a => a.User);
    }
}