using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebIcecream.DTOs
{
    public class NewRecipeDTO
    {
        [Required(ErrorMessage = "RecipeId is required.")]
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Flavor is required.")]
        public string Flavor { get; set; }

        [Required(ErrorMessage = "Ingredients are required.")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Procedure is required.")]
        public string Procedure { get; set; }

        public string ImageUrl { get; set; } 

        [Required(ErrorMessage = "SubmissionDate is required.")]
        public DateTime SubmissionDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }
    }

    public class UpdateNewRecipeDTO
    {
        [Required(ErrorMessage = "RecipeId is required.")]
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Flavor is required.")]
        public string Flavor { get; set; }

        [Required(ErrorMessage = "Ingredients are required.")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Procedure is required.")]
        public string Procedure { get; set; }

        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "SubmissionDate is required.")]
        public DateTime SubmissionDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }

        public IFormFile ImageFile { get; set; } 
    }
}
