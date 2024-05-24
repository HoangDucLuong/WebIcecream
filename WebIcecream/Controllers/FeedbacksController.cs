using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebIcecream.DTOs;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private static List<FeedbackDTO> _feedbacks = new List<FeedbackDTO>(); // In-memory store for feedbacks

        // GET: api/feedbacks
        [HttpGet]
        public ActionResult<IEnumerable<FeedbackDTO>> GetAllFeedbacks()
        {
            return Ok(_feedbacks);
        }

        // GET: api/feedbacks/{id}
        [HttpGet("{id}")]
        public ActionResult<FeedbackDTO> GetFeedbackById(int id)
        {
            var feedback = _feedbacks.FirstOrDefault(f => f.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return feedback;
        }

        // POST: api/feedbacks
        [HttpPost]
        public ActionResult<FeedbackDTO> CreateFeedback([FromBody] FeedbackDTO feedbackDto)
        {
            feedbackDto.FeedbackId = _feedbacks.Any() ? _feedbacks.Max(f => f.FeedbackId) + 1 : 1;
            feedbackDto.FeedbackDate = DateTime.UtcNow; // Set the feedback date to the current UTC time
            _feedbacks.Add(feedbackDto);
            return CreatedAtAction(nameof(GetFeedbackById), new { id = feedbackDto.FeedbackId }, feedbackDto);
        }

        // PUT: api/feedbacks/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateFeedback(int id, [FromBody] FeedbackDTO feedbackDto)
        {
            var index = _feedbacks.FindIndex(f => f.FeedbackId == id);
            if (index < 0)
            {
                return NotFound();
            }

            feedbackDto.FeedbackId = id; // Ensure the ID is correct
            _feedbacks[index] = feedbackDto;
            return NoContent();
        }

        // DELETE: api/feedbacks/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteFeedback(int id)
        {
            var feedback = _feedbacks.FirstOrDefault(f => f.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            _feedbacks.Remove(feedback);
            return NoContent();
        }
    }

}
