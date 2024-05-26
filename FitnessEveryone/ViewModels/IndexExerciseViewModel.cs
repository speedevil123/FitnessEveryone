using FitnessEveryone.Models;

namespace FitnessEveryone.ViewModels
{
    public class IndexExerciseViewModel
    {
        //Все упражнения
        public List<Exercise>? Exercises { get; set; }
        //Программа тренировок
        public int Height { get; set; }
        public int Weigth { get; set; }
        //Случ. упражнение
        public Exercise? Exercise { get; set; }


    }
}
