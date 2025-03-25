using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ContentMasterAPI.API.Models
{
    /// <summary>
    /// Represents a GraphQL query request
    /// </summary>
    public class GraphQLQuery
    {
        /// <summary>
        /// Name of the operation to execute
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// GraphQL query string
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Variables for the GraphQL query
        /// </summary>
        public Dictionary<string, object> Variables { get; set; }

        
    }

}