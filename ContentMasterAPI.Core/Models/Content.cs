using System;
using System.Collections.Generic;

namespace ContentMasterAPI.Core.Models
{
    /// <summary>
    /// Represents a content item in the ContentMaster API
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Unique identifier for the content
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the content
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Main body of the content
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Type of content (article, image, video, etc.)
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Date and time when the content was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the content was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// User who created the content
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Status of the content (draft, published, archived)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Tags associated with the content for categorization and search
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Custom metadata for the content stored as key-value pairs
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Version number of the content
        /// </summary>
        public int Version { get; set; }
    }
}
