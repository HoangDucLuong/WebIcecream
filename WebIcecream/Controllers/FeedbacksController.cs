using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebIcecream.DTOs;

namespace WebIcecream.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private static List<FeedbackDTO> _feedbacks = new List<FeedbackDTO>(); 

        [HttpGet]
        public ActionResult<IEnumerable<FeedbackDTO>> GetAllFeedbacks()
        {
            return Ok(_feedbacks);
        }

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

        [HttpPost]
        public ActionResult<FeedbackDTO> CreateFeedback([FromBody] FeedbackDTO feedbackDto)
        {
            feedbackDto.FeedbackId = _feedbacks.Any() ? _feedbacks.Max(f => f.FeedbackId) + 1 : 1;
            feedbackDto.FeedbackDate = DateTime.UtcNow; 
            _feedbacks.Add(feedbackDto);
            return CreatedAtAction(nameof(GetFeedbackById), new { id = feedbackDto.FeedbackId }, feedbackDto);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateFeedback(int id, [FromBody] FeedbackDTO feedbackDto)
        {
            var index = _feedbacks.FindIndex(f => f.FeedbackId == id);
            if (index < 0)
            {
                return NotFound();
            }

            feedbackDto.FeedbackId = id; 
            _feedbacks[index] = feedbackDto;
            return NoContent();
        }

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
