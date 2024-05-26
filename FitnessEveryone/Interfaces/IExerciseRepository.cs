using FitnessEveryone.Data.Enum;
using FitnessEveryone.Models;

namespace FitnessEveryone.Interfaces
{
    public interface IExerciseRepository
    {
        Task<IEnumerable<Exercise>> GetAll();
        Task<Exercise> GetByIdAsync(int id);
        Task<Exercise> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<Exercise>> GetExerciseByLocationBodyPart(BodyPartCategory bodyPartCategory, LocationCategory locationCategory);
        Task<IEnumerable<Exercise>> GetExerciseByLocationBodyPartNoCardio(BodyPartCategory bodyPartCategory, LocationCategory locationCategory);

        Task<IEnumerable<Exercise>> GetExerciseByBodyPart(BodyPartCategory bodyPartCategory);
        Task<IEnumerable<Exercise>> GetExerciseByBodyPartNoCardio(BodyPartCategory bodyPartCategory);

        Task<IEnumerable<Exercise>> GetExerciseByLocation(LocationCategory locationCategory);
        Task<IEnumerable<Exercise>> GetExerciseByDificulty(DificultyCategory dificultyCategory);

        Task<Exercise> GetByRandom(BodyPartCategory bodyPartCategory);
        Task<Exercise> GetByRandom(DificultyCategory dificultyCategory);

        bool Add(Exercise ex);
        bool Update(Exercise ex);
        bool Delete(Exercise ex);
        bool Save();
    }
}
