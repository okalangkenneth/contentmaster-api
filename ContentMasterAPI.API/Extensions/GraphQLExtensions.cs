using System.Collections.Generic;

namespace ContentMasterAPI.API.Extensions
{
    /// <summary>
    /// Provides extension methods for GraphQL operations.
    /// </summary>
    public static class GraphQLExtensions
    {
        /// <summary>
        /// Converts a dictionary of variables to GraphQL inputs.
        /// </summary>
        /// <param name="variables">The dictionary of variables to convert.</param>
        /// <returns>A dictionary representing the GraphQL inputs.</returns>
        public static Dictionary<string, object> ToInputs(this Dictionary<string, object> variables)
        {
            if (variables == null)
                return null;

            return variables;
        }
    }
}
