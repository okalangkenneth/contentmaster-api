// GraphQL/Types/ContentType.cs
using HotChocolate.Types;
using ContentMasterAPI.Core.Models;

namespace ContentMasterAPI.API.GraphQL.Types
{
    public class ContentType : ObjectType<Content>
    {
        protected override void Configure(IObjectTypeDescriptor<Content> descriptor)
        {
            descriptor.Description("A content item");

            descriptor.Field(c => c.Id).Description("The unique identifier of the content");
            descriptor.Field(c => c.Title).Description("The title of the content");
            descriptor.Field(c => c.Body).Description("The body of the content");
            descriptor.Field(c => c.ContentType).Description("The type of the content");
            descriptor.Field(c => c.CreatedAt).Description("When the content was created");
            descriptor.Field(c => c.UpdatedAt).Description("When the content was last updated");
            descriptor.Field(c => c.CreatedBy).Description("Who created the content");
            descriptor.Field(c => c.Status).Description("The status of the content");
            descriptor.Field(c => c.Tags).Description("Content tags");
            descriptor.Field(c => c.Metadata).Description("Content metadata");
        }
    }
}

