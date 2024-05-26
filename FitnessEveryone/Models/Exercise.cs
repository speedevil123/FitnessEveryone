using FitnessEveryone.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace FitnessEveryone.Models
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public LocationCategory LocationCategory { get; set; }
        public BodyPartCategory BodyPartCategory { get; set; }
        public DificultyCategory DificultyCategory { get; set; }
        public string QuantitySets { get; set; }
        public string QuantityReps { get; set; }
        public string Image {  get; set; }

    }
}
