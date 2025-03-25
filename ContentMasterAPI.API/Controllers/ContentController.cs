using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContentMasterAPI.Core.Interfaces;
using ContentMasterAPI.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContentMasterAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentRepository _contentRepository;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IContentRepository contentRepository, ILogger<ContentController> logger)
        {
            _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all content items
        /// </summary>
        /// <returns>A collection of all content items</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Content>>> GetAll()
        {
            try
            {
                var contents = await _contentRepository.GetAllAsync();
                return Ok(contents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all content");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Get a content item by ID
        /// </summary>
        /// <param name="id">The unique identifier of the content</param>
        /// <returns>The content item if found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Content>> GetById(Guid id)
        {
            try
            {
                var content = await _contentRepository.GetByIdAsync(id);
                if (content == null)
                {
                    return NotFound($"Content with ID {id} not found");
                }
                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving content with ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        /// <summary>
        /// Create a new content item
        /// </summary>
        /// <param name="content">The content item to create</param>
        /// <returns>The created content item</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Content>> Create([FromBody] Content content)
        {
            try
            {
                if (content == null)
                {
                    return BadRequest("Content object is null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                content.Id = Guid.NewGuid();
                content.CreatedAt = DateTime.UtcNow;
                content.UpdatedAt = DateTime.UtcNow;
                content.Version = 1;

                var createdContent = await _contentRepository.CreateAsync(content);
                return CreatedAtAction(nameof(GetById), new { id = createdContent.Id }, createdContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating content");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating content");
            }
        }

        /// <summary>
        /// Update an existing content item
        /// </summary>
        /// <param name="id">The unique identifier of the content to update</param>
        /// <param name="content">The updated content item</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] Content content)
        {
            try
            {
                if (content == null)
                {
                    return BadRequest("Content object is null");
                }

                if (id != content.Id)
                {
                    return BadRequest("Content ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingContent = await _contentRepository.GetByIdAsync(id);
                if (existingContent == null)
                {
                    return NotFound($"Content with ID {id} not found");
                }

                content.UpdatedAt = DateTime.UtcNow;
                content.Version = existingContent.Version + 1;
                content.CreatedAt = existingContent.CreatedAt;
                content.CreatedBy = existingContent.CreatedBy;

                await _contentRepository.UpdateAsync(content);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating content with ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating content");
            }
        }

        /// <summary>
        /// Delete a content item
        /// </summary>
        /// <param name="id">The unique identifier of the content to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var content = await _contentRepository.GetByIdAsync(id);
                if (content == null)
                {
                    return NotFound($"Content with ID {id} not found");
                }

                var result = await _contentRepository.DeleteAsync(id);
                if (!result)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting content");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting content with ID {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting content");
            }
        }

        /// <summary>
        /// Search for content items
        /// </summary>
        /// <param name="searchTerm">The search term to match against title and body</param>
        /// <param name="contentType">Optional content type filter</param>
        /// <param name="tags">Optional comma-separated tags to filter by</param>
        /// <returns>A collection of matching content items</returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Content>>> Search(
            [FromQuery] string searchTerm,
            [FromQuery] string contentType = null,
            [FromQuery] string tags = null)
        {
            try
            {
                List<string> tagsList = null;
                if (!string.IsNullOrEmpty(tags))
                {
                    tagsList = new List<string>(tags.Split(','));
                }

                var results = await _contentRepository.SearchAsync(searchTerm, contentType, tagsList);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for content with term {SearchTerm}", searchTerm);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error searching for content");
            }
        }
    }
}
