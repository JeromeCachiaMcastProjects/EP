using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.ViewModels
{
    public class PollCreateViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Option1Text { get; set; } = string.Empty;
        [Required]
        public string Option2Text { get; set; } = string.Empty;
        [Required]
        public string Option3Text { get; set; } = string.Empty;
    }
}
