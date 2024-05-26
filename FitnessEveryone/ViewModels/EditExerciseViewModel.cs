using FitnessEveryone.Data.Enum;

namespace FitnessEveryone.ViewModels
{
    public class EditExerciseViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public LocationCategory LocationCategory { get; set; }
        public BodyPartCategory BodyPartCategory { get; set; }
        public DificultyCategory DificultyCategory { get; set; }
        public string? QuantitySets { get; set; }
        public string? QuantityReps { get; set; }
        public IFormFile? Image { get; set; }
        public string? URL { get; set; }
    }
}
