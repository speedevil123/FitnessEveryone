using FitnessEveryone.Data.Enum;

namespace FitnessEveryone.ViewModels
{
    public class CalculationViewModel
    {
        public int Id { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        public Gender gender { get; set; }

        public double SNK { get; set; }
        //БЖУ
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }
        //ИМТ
        public double IMT { get; set; }
        public string? BodyWeight { get; set; }
        public double MCHS { get; set; }
        public double KOV { get; set; }
        public double PZT { get; set; }
    }
}
