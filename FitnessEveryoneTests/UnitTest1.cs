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
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Builder;

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
            var model = viewResult?.Model as Exercise;

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
                Exercise = new Exercise { Title = "123", BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym }
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
            var model = viewResult?.Model as IndexExerciseViewModel;

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
            var model = viewResult?.Model as List<Exercise>;
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
            Exercise Cardio1 = new Exercise() { Id = 1, Title = "Cardio", DificultyCategory = DificultyCategory.Cardio };
            Exercise Cardio2 = new Exercise() { Id = 2, Title = "Cardio", DificultyCategory = DificultyCategory.Cardio };
            Exercise Legs1 = new Exercise() { Id = 3, Title = "Legs", BodyPartCategory = BodyPartCategory.Legs };
            Exercise Legs2 = new Exercise() { Id = 4, Title = "Legs", BodyPartCategory = BodyPartCategory.Legs };

            Exercise Back1 = new Exercise() { Id = 5, Title = "Back", BodyPartCategory = BodyPartCategory.Back };
            Exercise Back2 = new Exercise() { Id = 6, Title = "Back", BodyPartCategory = BodyPartCategory.Back };

            Exercise Hands1 = new Exercise() { Id = 7, Title = "Hands", BodyPartCategory = BodyPartCategory.Hands };
            Exercise Hands2 = new Exercise() { Id = 8, Title = "Hands", BodyPartCategory = BodyPartCategory.Hands };

            Exercise Chest1 = new Exercise() { Id = 9, Title = "Chest", BodyPartCategory = BodyPartCategory.Chest };
            Exercise Chest2 = new Exercise() { Id = 10, Title = "Chest", BodyPartCategory = BodyPartCategory.Chest };


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
            var model = viewResult?.Model as IndexExerciseViewModel;

            List<Exercise> exercises = model.Exercises.Where(p => p.DificultyCategory == DificultyCategory.Cardio).ToList();
            int actual = exercises.Count();

            Assert.IsNotNull(model);
            Assert.AreEqual(expected, actual);


        }
        [TestMethod]
        public async Task RandomExercise()
        {
            //Arrange
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();
            _photoServiceMock = new Mock<IPhotoService>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise {Id = 1, BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym},
                new Exercise {BodyPartCategory = BodyPartCategory.Hands},
                new Exercise {BodyPartCategory = BodyPartCategory.Chest},
                new Exercise {BodyPartCategory = BodyPartCategory.Back},
                new Exercise {Id = 2, BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym},
                new Exercise {BodyPartCategory = BodyPartCategory.Legs, DificultyCategory = DificultyCategory.Cardio}
            };

            IndexExerciseViewModel IndexVM = new IndexExerciseViewModel()
            {
                Exercises = exercises,
                Exercise = new Exercise { BodyPartCategory = BodyPartCategory.Legs, LocationCategory = LocationCategory.Gym }
            };

            _exerciseRepositoryMock.Setup(m => m.GetExerciseByLocationBodyPartNoCardio(BodyPartCategory.Legs, LocationCategory.Gym))
                .ReturnsAsync(exercises
                .Where(p => p.BodyPartCategory == BodyPartCategory.Legs && p.LocationCategory == LocationCategory.Gym && p.DificultyCategory != DificultyCategory.Cardio)
                .ToList());
            //Act
            var controller = new ExerciseController(_exerciseRepositoryMock.Object, _photoServiceMock.Object);
            var result = await controller.RandomExercise(IndexVM);
            var redirectResult = result as RedirectToActionResult;

            //Assert
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Detail", redirectResult.ActionName);
            Assert.AreEqual("Exercise", redirectResult.ControllerName);
            Assert.IsTrue(redirectResult.RouteValues.Keys.Contains("id"));
            Assert.IsTrue(redirectResult.RouteValues["id"].Equals(1) || redirectResult.RouteValues["id"].Equals(2));
        }
        [TestMethod]
        public async Task CreatePostTest()
        {
            //Arrange
            Mock<IFormFile> _formFile = new Mock<IFormFile>();

            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_from", fileName);

            _exerciseRepositoryMock = new Mock<IExerciseRepository>();
            _photoServiceMock = new Mock<IPhotoService>();
            CreateExerciseViewModel createModel = new CreateExerciseViewModel()
            {
                Title = "Test Exercise",
                Description = "Description",
                LocationCategory = LocationCategory.Gym,
                BodyPartCategory = BodyPartCategory.Back,
                DificultyCategory = DificultyCategory.Cardio,
                QuantitySets = "3",
                QuantityReps = "10",
                Image = file // Use the helper method here
            };
            ImageUploadResult imageUpload = new ImageUploadResult()
            {
                Url = new Uri("https://www.example.com")
            };

            _photoServiceMock.Setup(m => m.AddPhotoAsync(file)).ReturnsAsync(imageUpload);

            var controller = new ExerciseController(_exerciseRepositoryMock.Object, _photoServiceMock.Object);
            var result = await controller.Create(createModel);
            var redirectResult = result as RedirectToActionResult;

            Assert.IsNotNull(redirectResult);
            Assert.IsInstanceOfType(redirectResult, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", redirectResult.ActionName);
            _exerciseRepositoryMock.Verify(x => x.Add(It.IsAny<Exercise>()), Times.Once);

        }
        [TestMethod]
        public async Task EditTest()
        {
            _exerciseRepositoryMock = new Mock<IExerciseRepository>();
            _photoServiceMock = new Mock<IPhotoService>();

            List<Exercise> exercises = new List<Exercise>()
            {
                new Exercise { Id = 1, Title = "Test123" },
                new Exercise { Id = 2, Title = "Test456" },
                new Exercise { Id = 3, Title = "Test789" },
            };
            int id = 2;
            _exerciseRepositoryMock?.Setup(m => m.GetByIdAsync(id)).ReturnsAsync(exercises.FirstOrDefault(p => p.Id == id));
            var controller = new ExerciseController(_exerciseRepositoryMock.Object, _photoServiceMock.Object);
            var result = await controller.Edit(id);
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as EditExerciseViewModel;

            Assert.IsNotNull(model);
            Assert.IsInstanceOfType(model, typeof(EditExerciseViewModel));
            Assert.AreEqual(exercises[1].Title, model.Title);
            Assert.AreNotEqual(exercises[1].Id, model.Id);

        }
    }
    [TestClass]
    public class AccountControllerTest : IClassFixture<TestDatabaseFixture>
    {
        private Mock<UserManager<AppUser>> _userManagerMock;
        private Mock<SignInManager<AppUser>> _signInManagerMock;
        public TestDatabaseFixture Fixture { get; }
        public AccountControllerTest()
        {
            Fixture = new TestDatabaseFixture();
        }

        public AccountControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [TestMethod]
        public void LoginTest()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null);
            var context = Fixture.CreateContext();

            LoginViewModel loginVM = new LoginViewModel();
            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, context);

            var result = controller.Login();
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as LoginViewModel;

            Assert.IsNotNull(viewResult);
            Assert.IsNull(model.EmailAddress);
            Assert.IsNull(model.Password);
        }
        [TestMethod]
        public async Task LoginPostPasswordCorrectTest()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null);
            var context = Fixture.CreateContext();
            LoginViewModel loginVM = new LoginViewModel()
            {
                Password = "123",
                EmailAddress = "nikita@mail.ru"
            };
            AppUser appUser = new AppUser()
            {
                Email = "nikita@mail.ru"
            };

            _userManagerMock.Setup(p => p.FindByEmailAsync(loginVM.EmailAddress))
                .ReturnsAsync(appUser);

            _userManagerMock.Setup(p => p.CheckPasswordAsync(It.Is<AppUser>(user =>
                    user.Email == loginVM.EmailAddress), loginVM.Password))
                .ReturnsAsync(true);

            _signInManagerMock.Setup(p => p.PasswordSignInAsync(It.Is<AppUser>(user =>
                    user.Email == loginVM.EmailAddress), loginVM.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, context);

            var redirectResult = await controller.Login(loginVM) as RedirectToActionResult;

            _signInManagerMock.Verify(p => p.PasswordSignInAsync(It.Is<AppUser>(user =>
                    user.Email == loginVM.EmailAddress), loginVM.Password, false, false), Times.Once());
            _userManagerMock.Verify(p => p.FindByEmailAsync(loginVM.EmailAddress), Times.Once());
            _userManagerMock.Verify(p => p.CheckPasswordAsync(It.Is<AppUser>(user =>
                    user.Email == loginVM.EmailAddress), loginVM.Password), Times.Once());

            Assert.IsNotNull(redirectResult);
            Assert.IsInstanceOfType(redirectResult, typeof(RedirectToActionResult));
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Exercise", redirectResult.ControllerName);
        }
        [TestMethod]
        public async Task LoginPostPasswordIncorrectTest()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null);
            var context = Fixture.CreateContext();
            LoginViewModel loginVM = new LoginViewModel()
            {
                Password = "123",
                EmailAddress = "nikita@mail.ru"
            };
            AppUser appUser = new AppUser()
            {
                Email = "nikita@mail.ru"
            };

            _userManagerMock.Setup(p => p.FindByEmailAsync(loginVM.EmailAddress))
                .ReturnsAsync(appUser);

            _userManagerMock.Setup(p => p.CheckPasswordAsync(It.Is<AppUser>(user =>
                    user.Email == loginVM.EmailAddress), loginVM.Password))
                .ReturnsAsync(false);



            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var redirectResult = await controller.Login(loginVM) as ViewResult;
            var model = redirectResult.Model as LoginViewModel;

            _signInManagerMock.Verify(p => p.PasswordSignInAsync(It.Is<AppUser>(user =>
                    user.Email == loginVM.EmailAddress), loginVM.Password, false, false), Times.Never());
            _userManagerMock.Verify(p => p.FindByEmailAsync(loginVM.EmailAddress), Times.Once());
            _userManagerMock.Verify(p => p.CheckPasswordAsync(It.Is<AppUser>(user =>
                    user.Email == loginVM.EmailAddress), loginVM.Password), Times.Once());   
            
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(loginVM.Password, model.Password);
            Assert.AreEqual(loginVM.EmailAddress, model.EmailAddress);


        }
        [TestMethod]
        public async Task LoginPostUserNotFoundTest()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null);
            var context = Fixture.CreateContext();
            LoginViewModel loginVM = new LoginViewModel()
            {
                Password = "123",
                EmailAddress = "nikita@mail.ru"
            };
            AppUser? appUser = null;

            _userManagerMock.Setup(p => p.FindByEmailAsync(loginVM.EmailAddress))
                .ReturnsAsync(appUser);

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var redirectResult = await controller.Login(loginVM) as ViewResult;
            var model = redirectResult.Model as LoginViewModel;

            _signInManagerMock.Verify(p => p.PasswordSignInAsync(It.Is<AppUser>(user =>
                    user.Email == loginVM.EmailAddress), loginVM.Password, false, false), Times.Never());
            _userManagerMock.Verify(p => p.FindByEmailAsync(loginVM.EmailAddress), Times.Once());
            _userManagerMock.Verify(p => p.CheckPasswordAsync(It.Is<AppUser>(user =>
                    user.Email == loginVM.EmailAddress), loginVM.Password), Times.Never());

            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(loginVM.Password, model.Password);
            Assert.AreEqual(loginVM.EmailAddress, model.EmailAddress);


        }
        [TestMethod]
        public void RegisterTest()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null);
            var context = Fixture.CreateContext();

            RegisterViewModel registerVM = new RegisterViewModel();
            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, context);

            var result = controller.Register();
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as RegisterViewModel;

            Assert.IsNotNull(viewResult);
            Assert.IsNull(model.EmailAddress);
            Assert.IsNull(model.Password);
            Assert.IsNull(model.ConfirmPassword);
        }
        [TestMethod]
        public async Task RegisterPostUserNotNullTest()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null);
            var context = Fixture.CreateContext();

            RegisterViewModel registerVM = new RegisterViewModel()
            {
                EmailAddress = "nikita@mail.ru",
                Password = "123",
                ConfirmPassword = "123"
            };
            AppUser appUser = new AppUser()
            {
                UserName = "nikita@mail.ru",
                Email = "nikita@mail.ru"
            };

            _userManagerMock.Setup(p => p.FindByEmailAsync(registerVM.EmailAddress))
                .ReturnsAsync(appUser);

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var viewResult = await controller.Register(registerVM) as ViewResult;
            var model = viewResult.Model as RegisterViewModel;

            _userManagerMock.Verify(p => p.FindByEmailAsync(registerVM.EmailAddress), Times.Once);
            _userManagerMock.Verify(p => p.AddToRoleAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), UserRoles.User), Times.Never);
            _userManagerMock.Verify(p => p.CreateAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), registerVM.Password), Times.Never);


            Assert.IsNotNull(model);
            Assert.AreEqual(registerVM.EmailAddress, model.EmailAddress);
        }
        [TestMethod]
        public async Task RegisterPostNewUserResponseSucceededTest()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null);
            var context = Fixture.CreateContext();

            RegisterViewModel registerVM = new RegisterViewModel()
            {
                EmailAddress = "nikita@mail.ru",
                Password = "123",
                ConfirmPassword = "123"
            };
            AppUser appNull = null;
            AppUser appUser = new AppUser()
            {
                UserName = "nikita@mail.ru",
                Email = "nikita@mail.ru"
            };
            IdentityResult resultIdentity = new IdentityResult();

            _userManagerMock.Setup(p => p.FindByEmailAsync(registerVM.EmailAddress))
                .ReturnsAsync(appNull);
            _userManagerMock.Setup(p => p.AddToRoleAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), UserRoles.User));
            _userManagerMock.Setup(p => p.CreateAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), registerVM.Password))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var redirectToActionResult = await controller
                .Register(registerVM) as RedirectToActionResult;

            _userManagerMock.Verify(p => p.FindByEmailAsync(registerVM.EmailAddress), Times.Once);
            _userManagerMock.Verify(p => p.AddToRoleAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), UserRoles.User), Times.Once);
            _userManagerMock.Verify(p => p.CreateAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), registerVM.Password), Times.Once);


            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Index",redirectToActionResult.ActionName);
            Assert.AreEqual("Exercise", redirectToActionResult.ControllerName);
        }
        [TestMethod]
        public async Task RegisterPostnNewUserResponseNotSucceededTest()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null);
            var context = Fixture.CreateContext();

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            RegisterViewModel registerVM = new RegisterViewModel()
            {
                EmailAddress = "nikita@mail.ru",
                Password = "123",
                ConfirmPassword = "123"
            };
            AppUser appNull = null;
            AppUser appUser = new AppUser()
            {
                UserName = "nikita@mail.ru",
                Email = "nikita@mail.ru"
            };
            IdentityResult resultIdentity = new IdentityResult();

            _userManagerMock.Setup(p => p.FindByEmailAsync(registerVM.EmailAddress))
                .ReturnsAsync(appNull);
            _userManagerMock.Setup(p => p.AddToRoleAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), UserRoles.User));
            _userManagerMock.Setup(p => p.CreateAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), registerVM.Password))
                .ReturnsAsync(IdentityResult.Failed());

            var redirectToActionResult = await controller
                .Register(registerVM) as RedirectToActionResult;

            _userManagerMock.Verify(p => p.FindByEmailAsync(registerVM.EmailAddress), Times.Once);
            _userManagerMock.Verify(p => p.AddToRoleAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), UserRoles.User), Times.Never);
            _userManagerMock.Verify(p => p.CreateAsync(It.Is<AppUser>(user =>
                    user.Email == registerVM.EmailAddress &&
                    user.UserName == registerVM.EmailAddress), registerVM.Password), Times.Once);


            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("Exercise", redirectToActionResult.ControllerName);
        }
        [TestMethod]
        public async Task LogoutTest()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(),
            null, null, null, null);
            var context = Fixture.CreateContext();

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, context);
            _signInManagerMock.Setup(p => p.SignOutAsync());

            var redirectToActionResult = await controller.Logout() as RedirectToActionResult;

            _signInManagerMock.Verify(p => p.SignOutAsync(), Times.Once);

            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("Exercise", redirectToActionResult.ControllerName);
            

        }
    }
}