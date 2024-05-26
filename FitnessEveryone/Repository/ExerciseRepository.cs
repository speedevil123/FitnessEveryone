using FitnessEveryone.Data;
using FitnessEveryone.Data.Enum;
using FitnessEveryone.Interfaces;
using FitnessEveryone.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessEveryone.Repository
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDbContext _context;
        public ExerciseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool Add(Exercise ex)
        {
            _context.Add(ex);
            return Save();
        }

        public bool Delete(Exercise ex)
        {
            _context.Remove(ex);
            return Save();
        }

        public async Task<IEnumerable<Exercise>> GetAll()
        {
            return await _context.Exercises.ToListAsync();
        }
        public async Task<IEnumerable<Exercise>> GetExerciseByLocationBodyPart(BodyPartCategory bodyPartCategory, LocationCategory locationCategory)
        {
            return await _context.Exercises.Where(c => c.BodyPartCategory == bodyPartCategory && c.LocationCategory == locationCategory).ToListAsync();
        }
        public async Task<IEnumerable<Exercise>> GetExerciseByLocationBodyPartNoCardio(BodyPartCategory bodyPartCategory, LocationCategory locationCategory)
        {
            return await _context.Exercises.Where(c => c.BodyPartCategory == bodyPartCategory && c.LocationCategory == locationCategory && c.DificultyCategory != DificultyCategory.Cardio).ToListAsync();
        }
        public async Task<Exercise> GetByIdAsync(int id)
        {
            return await _context.Exercises.FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<Exercise> GetByRandom(DificultyCategory dificultyCategory)
        {
            Random random = new Random();
            int count = 0;
            int index = 0;
            Exercise selected = default;

            var selectedExercises = await GetExerciseByDificulty(dificultyCategory);
            index = random.Next(selectedExercises.Count());
            foreach (Exercise element in selectedExercises)
            {
                if (count == index)
                {
                    selected = element;
                }
                count++;
            }
            return selected;
        }
        public async Task<Exercise> GetByRandom(BodyPartCategory bodyPartCategory)
        {
            Random random = new Random();
            int count = 0;
            int index = 0;
            Exercise selected = default;
            var selectedExercises = await GetExerciseByBodyPartNoCardio(bodyPartCategory);
            index = random.Next(selectedExercises.Count());
            foreach (Exercise element in selectedExercises)
            {
                if (count == index)
                {
                    selected = element;
                }
                count++;
            }
            return selected;
        }
        public async Task<Exercise> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Exercises.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Exercise>> GetExerciseByLocation(LocationCategory location)
        {
            return await _context.Exercises.Where(c => c.LocationCategory == location).ToListAsync();
        }

        public async Task<IEnumerable<Exercise>> GetExerciseByBodyPart(BodyPartCategory bodyPartCategory)
        {
            return await _context.Exercises.Where(c => c.BodyPartCategory == bodyPartCategory).ToListAsync();
        }
        public async Task<IEnumerable<Exercise>> GetExerciseByBodyPartNoCardio(BodyPartCategory bodyPartCategory)
        {
            return await _context.Exercises.Where(c => c.BodyPartCategory == bodyPartCategory && c.DificultyCategory != DificultyCategory.Cardio).ToListAsync();
        }

        public async Task<IEnumerable<Exercise>> GetExerciseByDificulty(DificultyCategory dificultyCategory)
        {
            return await _context.Exercises.Where(c => c.DificultyCategory == dificultyCategory).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Exercise ex)
        {
            _context.Update(ex);
            return Save();
        }
    }
}
