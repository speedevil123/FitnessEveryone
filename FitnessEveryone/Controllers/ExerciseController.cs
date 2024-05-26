using FitnessEveryone.Data;
using FitnessEveryone.Data.Enum;
using FitnessEveryone.Interfaces;
using FitnessEveryone.Models;
using FitnessEveryone.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;

namespace FitnessEveryone.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly IExerciseRepository _ExerciseRepository;
        private readonly IPhotoService _photoService;

        public ExerciseController(IExerciseRepository ExerciseRepository, IPhotoService photoService)
        {
            _ExerciseRepository = ExerciseRepository;
            _photoService = photoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Exercise exercise = await _ExerciseRepository.GetByIdAsync(id);
            return View(exercise);
        }

        [HttpPost]
        public async Task<IActionResult> ListExercise(IndexExerciseViewModel model)
        {
            var selectedExercises = await _ExerciseRepository.GetExerciseByLocationBodyPart(model.Exercise.BodyPartCategory, model.Exercise.LocationCategory);
            var IndexExerciseVM = new IndexExerciseViewModel
            {
                Exercise = model.Exercise,
                Exercises = selectedExercises.ToList()
            };
            return View(IndexExerciseVM);
        }

        public async Task<IActionResult> AllExercise()
        {
            var result = await _ExerciseRepository.GetAll();
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> WorkoutProgram(IndexExerciseViewModel model)
        {
            double Height = (double)model.Height / 100;
            double result = (double)model.Weigth / (Height * Height);
            Random random = new Random();

            Exercise Cardio1 = new Exercise();
            Exercise Cardio2 = new Exercise();
            Exercise Legs1 = new Exercise();
            Exercise Legs2 = new Exercise();
            Exercise Back1 = new Exercise();
            Exercise Back2 = new Exercise();
            Exercise Hands1 = new Exercise();
            Exercise Hands2 = new Exercise();
            Exercise Chest1 = new Exercise();
            Exercise Chest2 = new Exercise();

            List<Exercise> exercises = new List<Exercise>();
            if (result < 18.5)
            {
                Legs1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Legs);
                do
                {
                    Legs2 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Legs);
                } while (Legs1.Id == Legs2.Id);

                Back1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Back);
                Hands1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Hands);
                Chest1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Chest);

                exercises.Add(Legs1);
                exercises.Add(Legs2);
                exercises.Add(Back1);
                exercises.Add(Hands1);
                exercises.Add(Chest1);

                var IndexExerciseVM = new IndexExerciseViewModel
                {
                    Exercises = exercises
                };
                return View(IndexExerciseVM);
            }
            if(18.5 <= result && result < 25)
            {
                Cardio1 = await _ExerciseRepository.GetByRandom(DificultyCategory.Cardio);
                Legs1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Legs);
                do
                {
                    Legs2 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Legs);
                } while (Legs1.Id == Legs2.Id);

                Back1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Back);
                do
                {
                    Back2 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Back);
                } while (Back1.Id == Back2.Id);

                Hands1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Hands);
                do
                {
                    Hands2 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Hands);
                } while (Hands1.Id == Hands2.Id);

                Chest1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Chest);
                do
                {
                    Chest2 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Chest);
                } while (Chest1.Id == Chest2.Id);

                exercises.Add(Cardio1);
                exercises.Add(Legs1);
                exercises.Add(Legs2);
                exercises.Add(Back1);
                exercises.Add(Back2);
                exercises.Add(Hands1);
                exercises.Add(Hands2);
                exercises.Add(Chest1);
                exercises.Add(Chest2);

                var IndexExerciseVM = new IndexExerciseViewModel
                {
                    Exercises = exercises
                };
                return View(IndexExerciseVM);
            }
            if(25 <= result && result < 30)
            {
                Cardio1 = await _ExerciseRepository.GetByRandom(DificultyCategory.Cardio);
                do
                {
                    Cardio2 = await _ExerciseRepository.GetByRandom(DificultyCategory.Cardio);
                } while (Cardio1.Id == Cardio2.Id);

                Legs1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Legs);
                do
                {
                    Legs2 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Legs);
                } while (Legs1.Id == Legs2.Id);

                Back1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Back);
                do
                {
                    Back2 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Back);
                } while (Back1.Id == Back2.Id);
                Hands1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Hands);
                Chest1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Chest);
                exercises.Add(Cardio1);
                exercises.Add(Legs1);
                exercises.Add(Legs2);
                exercises.Add(Back1);
                exercises.Add(Back2);
                exercises.Add(Hands1);
                exercises.Add(Chest1);
                exercises.Add(Cardio2);

                var IndexExerciseVM = new IndexExerciseViewModel
                {
                    Exercises = exercises
                };
                return View(IndexExerciseVM);
            }
            if(result >= 30)
            {
                Cardio1 = await _ExerciseRepository.GetByRandom(DificultyCategory.Cardio);
                do
                {
                    Cardio2 = await _ExerciseRepository.GetByRandom(DificultyCategory.Cardio);
                } while (Cardio1.Id == Cardio2.Id);

                Legs1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Legs);
                Back1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Back);
                Hands1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Hands);
                Chest1 = await _ExerciseRepository.GetByRandom(BodyPartCategory.Chest);

                exercises.Add(Cardio1);
                exercises.Add(Legs1);
                exercises.Add(Back1);
                exercises.Add(Hands1);
                exercises.Add(Chest1);
                exercises.Add(Cardio2);

                var IndexExerciseVM = new IndexExerciseViewModel
                {
                    Exercises = exercises
                };
                return View(IndexExerciseVM);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RandomExercise(IndexExerciseViewModel model)
        {
            Random random = new Random();
            int count = 0;
            int index = 0;
            Exercise selected = default;

            var selectedExercises = await _ExerciseRepository.GetExerciseByLocationBodyPartNoCardio(model.Exercise.BodyPartCategory, model.Exercise.LocationCategory);
            index = random.Next(selectedExercises.Count());
            foreach(Exercise element in selectedExercises)
            {
                if(count == index)
                {
                    selected = element;
                }
                count++;
            }
            int id;
            id = selected.Id;
            return RedirectToAction("Detail", "Exercise", new {id = id});
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExerciseViewModel exVM)
        {
            if(ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(exVM.Image);

                var ex = new Exercise
                {
                    Title = exVM.Title,
                    Description = exVM.Description,
                    LocationCategory = exVM.LocationCategory,
                    BodyPartCategory = exVM.BodyPartCategory,
                    DificultyCategory = exVM.DificultyCategory,
                    QuantitySets = exVM.QuantitySets,
                    QuantityReps = exVM.QuantityReps,
                    Image = result.Url.ToString()
                    

                };
                _ExerciseRepository.Add(ex);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo upload failed");
            }
            return View(exVM);
        }

        public async Task<IActionResult>Edit(int id)
        {
            var ex = await _ExerciseRepository.GetByIdAsync(id);
            if (ex == null) return View("Error");
            var exVM = new EditExerciseViewModel
            {
                Title = ex.Title,
                Description = ex.Description,
                LocationCategory = ex.LocationCategory,
                BodyPartCategory = ex.BodyPartCategory,
                DificultyCategory = ex.DificultyCategory,
                QuantitySets = ex.QuantitySets,
                QuantityReps = ex.QuantityReps,
                URL = ex.Image
            };
            return View(exVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditExerciseViewModel exVM)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit exercise");
                return View("Edit", exVM);
            }
            var userEx = await _ExerciseRepository.GetByIdAsyncNoTracking(id);
            if(userEx != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userEx.Image);
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(exVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(exVM.Image);

                var exercise = new Exercise
                {
                    Id = id,
                    Title = exVM.Title,
                    Description = exVM.Description,
                    QuantitySets = exVM.QuantitySets,
                    QuantityReps = exVM.QuantityReps,
                    LocationCategory = exVM.LocationCategory,
                    BodyPartCategory = exVM.BodyPartCategory,
                    DificultyCategory = exVM.DificultyCategory,
                    Image = photoResult.Url.ToString()

                };
                _ExerciseRepository.Update(exercise);
                return RedirectToAction("Index");
            }
            else
            {
                return View(exVM);
            }
        }
    }
}
