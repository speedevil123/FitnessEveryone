using FitnessEveryone.Controllers;
using FitnessEveryone.Data;
using FitnessEveryone.Interfaces;
using FitnessEveryone.Models;
using FitnessEveryone.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using Xunit;
using FitnessEveryone.Data.Enum;
using System.Security.Cryptography;

namespace FitnessEveryoneTests
{
    public class TestDatabaseFixture
    {
        //Задаем строку подключения БД
        private const string ConnectionString = @"Data Source=SPEEDEVIL\SQLEXPRESS;Initial Catalog=FitnessEveryoneTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        //Инициализация тестовой БД
        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public ApplicationDbContext CreateContext()
            => new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);
    }
    [TestClass]
    public class IExerciseRepositoryTest
    {
        private Mock<IExerciseRepository> _exerciseRepositoryMock;

        [TestMethod]
        public async Task GetAllTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise { },
                new Exercise { },
                new Exercise { },
                new Exercise { }
            };

            _exerciseRepositoryMock.Setup(m => m.GetAll()).ReturnsAsync(exercises);
            //Act
            var result = await _exerciseRepositoryMock.Object.GetAll();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public async Task GetExerciseByLocationBodyPartTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym},
                new Exercise {BodyPartCategory = BodyPartCategory.Back, LocationCategory = LocationCategory.Gym},
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Home },
                new Exercise {BodyPartCategory = BodyPartCategory.Back, LocationCategory= LocationCategory.Home },
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym }
            };

            _exerciseRepositoryMock.Setup(m => m.GetExerciseByLocationBodyPart(BodyPartCategory.Legs, LocationCategory.Gym))
                .ReturnsAsync(exercises
                .Where(p => p.BodyPartCategory == BodyPartCategory.Legs && p.LocationCategory == LocationCategory.Gym)
                .ToList());
            //Act
            var result = await _exerciseRepositoryMock.Object.GetExerciseByLocationBodyPart(BodyPartCategory.Legs, LocationCategory.Gym);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetExerciseByLocationBodyPartNoCardioTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym},
                new Exercise {BodyPartCategory = BodyPartCategory.Back, LocationCategory = LocationCategory.Gym},
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Home },
                new Exercise {BodyPartCategory = BodyPartCategory.Back, LocationCategory= LocationCategory.Home },
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym },
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym, DificultyCategory = DificultyCategory.Cardio }
            };

            _exerciseRepositoryMock.Setup(m => m.GetExerciseByLocationBodyPartNoCardio(BodyPartCategory.Legs, LocationCategory.Gym))
                .ReturnsAsync(exercises.Where(p => p.BodyPartCategory == BodyPartCategory.Legs &&
            p.LocationCategory == LocationCategory.Gym &&
            p.DificultyCategory != DificultyCategory.Cardio).ToList());
            //Act
            var result = await _exerciseRepositoryMock.Object.GetExerciseByLocationBodyPartNoCardio(BodyPartCategory.Legs, LocationCategory.Gym);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetByIdAsyncTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise { Id = 1 },
                new Exercise { Id = 2 },
                new Exercise { Id = 3 }
            };

            _exerciseRepositoryMock.Setup(m => m.GetByIdAsync(1))
                .ReturnsAsync(exercises.FirstOrDefault(p => p.Id == 1));
            //Act
            var result = await _exerciseRepositoryMock.Object.GetByIdAsync(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public async Task GetByRandomDifficultyCategoryTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {DificultyCategory = DificultyCategory.Functional },
                new Exercise {DificultyCategory = DificultyCategory.Strength },
                new Exercise {DificultyCategory = DificultyCategory.Cardio, Title = "Test_True" },
                new Exercise {DificultyCategory = DificultyCategory.Cardio, Title = "Test_False" }
            };
            _exerciseRepositoryMock.Setup(m => m.GetByRandom(DificultyCategory.Cardio))
                .ReturnsAsync(exercises.Where(p => p.DificultyCategory == DificultyCategory.Cardio)
                .ToList()[0]);
            //Act
            var result = await _exerciseRepositoryMock.Object.GetByRandom(DificultyCategory.Cardio);
            Assert.IsNotNull(result);
            Assert.AreEqual("Test_True", result.Title);
        }
        [TestMethod]
        public async Task GetByRandomBodyPartCategoryTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {BodyPartCategory = BodyPartCategory.Legs },
                new Exercise {BodyPartCategory = BodyPartCategory.Hands },
                new Exercise {BodyPartCategory = BodyPartCategory.Back, Title = "Test_True" },
                new Exercise {BodyPartCategory = BodyPartCategory.Back, Title = "Test_False" }
            };
            _exerciseRepositoryMock.Setup(m => m.GetByRandom(BodyPartCategory.Back))
                .ReturnsAsync(exercises.Where(p => p.BodyPartCategory == BodyPartCategory.Back)
                .ToList()[0]);
            //Act
            var result = await _exerciseRepositoryMock.Object.GetByRandom(BodyPartCategory.Back);
            Assert.IsNotNull(result);
            Assert.AreEqual("Test_True", result.Title);
        }
        [TestMethod]
        public async Task GetExerciseByLocationTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {LocationCategory = LocationCategory.Gym},
                new Exercise {LocationCategory = LocationCategory.Gym},
                new Exercise {LocationCategory = LocationCategory.Home },
                new Exercise {LocationCategory= LocationCategory.Street },
                new Exercise {LocationCategory = LocationCategory.Gym }
            };

            _exerciseRepositoryMock.Setup(m => m.GetExerciseByLocation(LocationCategory.Gym))
                .ReturnsAsync(exercises
                .Where(p => p.LocationCategory == LocationCategory.Gym)
                .ToList());
            //Act
            var result = await _exerciseRepositoryMock.Object.GetExerciseByLocation(LocationCategory.Gym);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }
        [TestMethod]
        public async Task GetExerciseByBodyPartTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {BodyPartCategory = BodyPartCategory.Legs},
                new Exercise {BodyPartCategory = BodyPartCategory.Hands},
                new Exercise {BodyPartCategory = BodyPartCategory.Chest},
                new Exercise {BodyPartCategory = BodyPartCategory.Back},
                new Exercise {BodyPartCategory = BodyPartCategory.Legs}
            };

            _exerciseRepositoryMock.Setup(m => m.GetExerciseByBodyPart(BodyPartCategory.Legs))
                .ReturnsAsync(exercises
                .Where(p => p.BodyPartCategory == BodyPartCategory.Legs)
                .ToList());
            //Act
            var result = await _exerciseRepositoryMock.Object.GetExerciseByBodyPart(BodyPartCategory.Legs);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
        [TestMethod]
        public async Task GetExerciseByBodyPartNoCardioTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {BodyPartCategory = BodyPartCategory.Legs},
                new Exercise {BodyPartCategory = BodyPartCategory.Hands},
                new Exercise {BodyPartCategory = BodyPartCategory.Chest},
                new Exercise {BodyPartCategory = BodyPartCategory.Back},
                new Exercise {BodyPartCategory = BodyPartCategory.Legs},
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, DificultyCategory = DificultyCategory.Cardio}
            };

            _exerciseRepositoryMock.Setup(m => m.GetExerciseByBodyPartNoCardio(BodyPartCategory.Legs))
                .ReturnsAsync(exercises
                .Where(p => p.BodyPartCategory == BodyPartCategory.Legs && p.DificultyCategory != DificultyCategory.Cardio)
                .ToList());
            //Act
            var result = await _exerciseRepositoryMock.Object.GetExerciseByBodyPartNoCardio(BodyPartCategory.Legs);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
        [TestMethod]
        public async Task GetExerciseByDifficultyTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {DificultyCategory = DificultyCategory.Cardio},
                new Exercise {DificultyCategory = DificultyCategory.Functional},
                new Exercise {DificultyCategory = DificultyCategory.Strength},
                new Exercise {DificultyCategory = DificultyCategory.Cardio},
                new Exercise {DificultyCategory = DificultyCategory.Cardio}
            };

            _exerciseRepositoryMock.Setup(m => m.GetExerciseByDificulty(DificultyCategory.Cardio))
                .ReturnsAsync(exercises
                .Where(p => p.DificultyCategory == DificultyCategory.Cardio)
                .ToList());
            //Act
            var result = await _exerciseRepositoryMock.Object.GetExerciseByDificulty(DificultyCategory.Cardio);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }
    }
    [TestClass]
    public class CalculationControllerTest : IClassFixture<TestDatabaseFixture>
    {

        [TestMethod]
        public void PZTTest()
        {
            //Arrange
            CalculationViewModel calcVM = new CalculationViewModel()
            {
                Age = 20
            };

            //Act
            var controller = new CalculationController();
            var result = controller.PZT(calcVM);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as CalculationViewModel;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(150, model.PZT);
        }

        [TestMethod]
        public void KOVTest()
        {
            //Arrange
            CalculationViewModel calcVM = new CalculationViewModel()
            {
                Weight = 100
            };

            //Act
            var controller = new CalculationController();
            var result = controller.KOV(calcVM);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as CalculationViewModel;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(2530, model.KOV);
        }

        [TestMethod]
        public void MCHSTest()
        {
            //Arrange
            CalculationViewModel calcVM = new CalculationViewModel()
            {
                Age = 20
            };

            //Act
            var controller = new CalculationController();
            var result = controller.MCHS(calcVM);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as CalculationViewModel;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(200, model.MCHS);
        }

        [TestMethod]
        public void BJUTest()
        {
            //Arrange
            CalculationViewModel calcVM = new CalculationViewModel()
            {
                Weight = 100
            };

            //Act
            var controller = new CalculationController();
            var result = controller.BJU(calcVM);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as CalculationViewModel;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(200, model.Proteins);
            Assert.AreEqual(150, model.Fats);
            Assert.AreEqual(400, model.Carbohydrates);
        }

        [TestMethod]
        public void SNKTest()
        {
            //Arrange
            CalculationViewModel calcVM = new CalculationViewModel()
            {
                Weight = 79,
                gender = FitnessEveryone.Data.Enum.Gender.Male,
                Age = 20,
                Height = 183
            };

            //Act
            var controller = new CalculationController();
            var result = controller.SNK(calcVM);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as CalculationViewModel;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(1838.75, model.SNK);
        }
        [TestMethod]
        public void IMTTest()
        {
            //Arrange
            CalculationViewModel calcVM = new CalculationViewModel()
            {
                Weight = 79,
                Height = 183
            };

            //Act
            var controller = new CalculationController();
            var result = controller.IMT(calcVM);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as CalculationViewModel;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(23.59, model.IMT);
            Assert.AreEqual("Normal", model.BodyWeight);
        }
    }
    [TestClass]
    public class ExerciseControllerTest : IClassFixture<TestDatabaseFixture>
    {
        private Mock<IExerciseRepository> _exerciseRepositoryMock;
        private Mock<IPhotoService> _photoServiceMock;

        public TestDatabaseFixture Fixture { get; }
        public ExerciseControllerTest()
        {
            Fixture = new TestDatabaseFixture();
        }

        public ExerciseControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [TestMethod]
        public async Task DetailTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();
            _photoServiceMock = new Mock<IPhotoService>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise { Id = 1 },
                new Exercise { Id = 2 },
                new Exercise { Id = 3 }
            };

            _exerciseRepositoryMock.Setup(m => m.GetByIdAsync(1))
                .ReturnsAsync(exercises.FirstOrDefault(p => p.Id == 1));

            var controller = new ExerciseController(_exerciseRepositoryMock.Object, _photoServiceMock.Object);
            var result = await controller.Detail(1);
            var viewResult = result as ViewResult;
            var model = viewResult.Model as Exercise;

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Id);
        }
        [TestMethod]
        public async Task ListExerciseTest()
        {
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();
            _photoServiceMock = new Mock<IPhotoService>();

            IndexExerciseViewModel IndexExVM = new IndexExerciseViewModel()
            {
                Exercise = new Exercise {Title = "123", BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym }
            };

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym},
                new Exercise {BodyPartCategory = BodyPartCategory.Back, LocationCategory = LocationCategory.Gym},
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Home },
                new Exercise {BodyPartCategory = BodyPartCategory.Back, LocationCategory= LocationCategory.Home },
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym }
            };

            _exerciseRepositoryMock.Setup(m => m.GetExerciseByLocationBodyPart(BodyPartCategory.Legs, LocationCategory.Gym))
                .ReturnsAsync(exercises
                .Where(p => p.BodyPartCategory == BodyPartCategory.Legs && p.LocationCategory == LocationCategory.Gym)
                .ToList());

            var controller = new ExerciseController(_exerciseRepositoryMock.Object, _photoServiceMock.Object);
            var result = await controller.ListExercise(IndexExVM);
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IndexExerciseViewModel;

            Assert.IsNotNull(model);
            Assert.AreEqual("123", model.Exercise.Title);
            Assert.AreEqual(2, model.Exercises.Count);
        }

        [TestMethod]
        public async Task AllExerciseTest()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();
            _photoServiceMock = new Mock<IPhotoService>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise { },
                new Exercise { },
                new Exercise { },
                new Exercise { }
            };

            _exerciseRepositoryMock.Setup(m => m.GetAll()).ReturnsAsync(exercises);
            //Act
            var controller = new ExerciseController(_exerciseRepositoryMock.Object, _photoServiceMock.Object);
            var result = await controller.AllExercise();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<Exercise>;
            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(4, model.Count());
        }
        public static IEnumerable<object[]> AdditionData1
        {
            get
            {
                return new[]
                {
                    new object[] {new IndexExerciseViewModel { Height = 180, Weigth = 40}, 0 },
                    new object[] {new IndexExerciseViewModel { Height = 180, Weigth = 75}, 1 },
                    new object[] {new IndexExerciseViewModel { Height = 180, Weigth = 180},2 }
                };
            }
        }
        [TestMethod]
        [DynamicData(nameof(AdditionData1))]
        public async Task WorkoutProgramTest(IndexExerciseViewModel indexVM, int expected)
        {
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();
            _photoServiceMock = new Mock<IPhotoService>();
            Exercise Cardio1 = new Exercise() {Id = 1, Title = "Cardio", DificultyCategory = DificultyCategory.Cardio };
            Exercise Cardio2 = new Exercise() {Id = 2, Title = "Cardio", DificultyCategory = DificultyCategory.Cardio };
            Exercise Legs1 = new Exercise() {Id = 3, Title = "Legs", BodyPartCategory = BodyPartCategory.Legs };
            Exercise Legs2 = new Exercise() {Id = 4, Title = "Legs", BodyPartCategory = BodyPartCategory.Legs };

            Exercise Back1 = new Exercise() {Id = 5, Title = "Back", BodyPartCategory = BodyPartCategory.Back };
            Exercise Back2 = new Exercise() {Id = 6, Title = "Back", BodyPartCategory = BodyPartCategory.Back };

            Exercise Hands1 = new Exercise() {Id = 7, Title = "Hands", BodyPartCategory = BodyPartCategory.Hands };
            Exercise Hands2 = new Exercise() {Id = 8, Title = "Hands", BodyPartCategory = BodyPartCategory.Hands };

            Exercise Chest1 = new Exercise() {Id = 9, Title = "Chest", BodyPartCategory = BodyPartCategory.Chest };
            Exercise Chest2 = new Exercise() {Id = 10, Title = "Chest", BodyPartCategory = BodyPartCategory.Chest };


            _exerciseRepositoryMock.SetupSequence(m => m.GetByRandom(DificultyCategory.Cardio))
                .ReturnsAsync(Cardio1)
                .ReturnsAsync(Cardio2);
            _exerciseRepositoryMock.SetupSequence(m => m.GetByRandom(BodyPartCategory.Legs))
                .ReturnsAsync(Legs1)
                .ReturnsAsync(Legs2);
            _exerciseRepositoryMock.SetupSequence(m => m.GetByRandom(BodyPartCategory.Back))
                .ReturnsAsync(Back1)
                .ReturnsAsync(Back2);
            _exerciseRepositoryMock.SetupSequence(m => m.GetByRandom(BodyPartCategory.Hands))
                .ReturnsAsync(Hands1)
                .ReturnsAsync(Hands2);
            _exerciseRepositoryMock.SetupSequence(m => m.GetByRandom(BodyPartCategory.Chest))
                .ReturnsAsync(Chest1)
                .ReturnsAsync(Chest2);

            var controller = new ExerciseController(_exerciseRepositoryMock.Object, _photoServiceMock.Object);
            var result = await controller.WorkoutProgram(indexVM);
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IndexExerciseViewModel;

            List<Exercise> exercises = model.Exercises.Where(p => p.DificultyCategory == DificultyCategory.Cardio).ToList();
            int actual = exercises.Count();

            Assert.IsNotNull(model);
            Assert.AreEqual(expected, actual);
   

        }
    }
}