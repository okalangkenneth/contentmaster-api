using Microsoft.AspNetCore.Mvc;

namespace ContentMasterAPI.API.Controllers
{
    // Note: With HotChocolate, you don't need a GraphQLController
    // HotChocolate automatically creates the endpoint at /graphql
    // This controller can be deleted

    // If you want to keep it for backward compatibility:
    [ApiController]
    [Route("api/graphql")]
    public class GraphQLController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            // Redirect to the HotChocolate endpoint
            return RedirectPermanent("/graphql");
        }
    }
}
