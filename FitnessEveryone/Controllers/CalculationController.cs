using FitnessEveryone.Data.Enum;
using FitnessEveryone.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static System.Math;

namespace FitnessEveryone.Controllers
{
    public class CalculationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //PZT
        public IActionResult PZT()
        {
            return View(new CalculationViewModel());
        }

        [HttpPost]
        public IActionResult PZT(CalculationViewModel calcVM)
        {
            if (ModelState.IsValid)
            {
                calcVM.MCHS = 220 - (double)calcVM.Age;
                calcVM.PZT = Math.Round((double)calcVM.MCHS * 0.75, 2);
                return View(calcVM);
            }
            else
            {
                return View(calcVM);
            }
        }
        //KOV
        public IActionResult KOV()
        {
            return View(new CalculationViewModel());
        }

        [HttpPost]
        public IActionResult KOV(CalculationViewModel calcVM)
        {
            if (ModelState.IsValid)
            {
                calcVM.KOV = Math.Round(370 + (21.6 * (double)calcVM.Weight), 2);

                return View(calcVM);
            }
            else
            {
                return View(calcVM);
            }
        }
        //MCHS
        public IActionResult MCHS()
        {
            return View(new CalculationViewModel());
        }

        [HttpPost]
        public IActionResult MCHS(CalculationViewModel calcVM)
        {
            if (ModelState.IsValid)
            {
                calcVM.MCHS = 220 - (double)calcVM.Age;

                return View(calcVM);
            }
            else
            {
                return View(calcVM);
            }
        }

        //BJU
        public IActionResult BJU()
        {
            return View(new CalculationViewModel());
        }

        [HttpPost]
        public IActionResult BJU(CalculationViewModel calcVM)
        {
            if (ModelState.IsValid)
            {
                calcVM.Proteins = Math.Round((double)calcVM.Weight * 2,3);
                calcVM.Fats = Math.Round((double)calcVM.Weight * 1.5,3);
                calcVM.Carbohydrates = Math.Round((double)calcVM.Weight * 4,3);

                return View(calcVM);
            }
            else
            {
                return View(calcVM);
            }
        }
        //SNK
        public IActionResult SNK()
        {
            return View(new CalculationViewModel()); 
        }

        [HttpPost]
        public IActionResult SNK(CalculationViewModel calcVM)
        {
            if (ModelState.IsValid)
            {
                if (calcVM.gender == Gender.Male)
                {
                    calcVM.SNK = Math.Round((10 * calcVM.Weight) + (6.25 * calcVM.Height) - (5 * calcVM.Age) + 5,3);
                }
                else
                {
                    calcVM.SNK = Math.Round((10 * calcVM.Weight) + (6.25 * calcVM.Height) - (5 * calcVM.Age) - 161,3);
                }

                return View("SNK", calcVM); 
            }
            else
            {
                return View(calcVM); 
            }
        }


        //IMT
        public IActionResult IMT()
        {
            return View(new CalculationViewModel());
        }

        [HttpPost]
        public IActionResult IMT(CalculationViewModel calcVM)
        {
            if (ModelState.IsValid)
            {
                double Height = (double)calcVM.Height / 100;
                calcVM.IMT = Math.Round((double)calcVM.Weight / (Height * Height),3);
                if (calcVM.IMT < 16)
                    calcVM.BodyWeight = "Significant Body Mass Deficit";
                if (16 <= calcVM.IMT && calcVM.IMT < 18.5)
                    calcVM.BodyWeight = "Body Mass Deficit";
                if (18 <= calcVM.IMT && calcVM.IMT < 25)
                    calcVM.BodyWeight = "Normal";
                if (25 <= calcVM.IMT && calcVM.IMT < 30)
                    calcVM.BodyWeight = "Overweight";
                if (30 <= calcVM.IMT && calcVM.IMT < 35)
                    calcVM.BodyWeight = "First-Degree Obesity";
                if (35 <= calcVM.IMT && calcVM.IMT <= 40)
                    calcVM.BodyWeight = "Second-Degree Obesity";
                if (calcVM.IMT > 40)
                    calcVM.BodyWeight = "Third-Degree Obesity";
                return View(calcVM);
            }
            else
            {
                return View(calcVM);
            }
        }
    }
}
