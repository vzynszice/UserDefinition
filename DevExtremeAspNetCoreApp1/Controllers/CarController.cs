using Microsoft.AspNetCore.Mvc;
using DAL.Exceptions;
using SystemManager.Abstractions.Car;
using System.Text.Json;

namespace DevExtremeAspNetCoreApp1.Controllers
{
    [Route("[controller]")]
    public class CarController : Controller
    {
        private readonly ICarDamageManager _carDamageManager;
        private readonly ILogger<CarController> _logger;

        public CarController(
            ICarDamageManager carDamageManager,
            ILogger<CarController> logger)
        {
            _carDamageManager = carDamageManager;
            _logger = logger;
        }

        [HttpGet("CarLogin")]
        public IActionResult CarLogin()
        {
            return View("~/Views/CarDamage/CarLogin.cshtml");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                throw new ValidationException("Plate number is required");
            }

            var cars = await _carDamageManager.GetCarsByPlateAsync(plateNumber);
            var car = cars.FirstOrDefault();

            if (car == null)
            {
                throw new NotFoundException("Car not found");
            }

            return RedirectToAction("DamageRecording", new { id = car.id });
        }

        [HttpGet("DamageRecording/{id}")]
        public async Task<IActionResult> DamageRecording(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException("Invalid car id");
            }

            var car = await _carDamageManager.GetCarDamagesAsync(id);

            ViewData["CarPlate"] = car.Plate;

            var carData = new
            {
                id = car.id,
                plate = car.Plate,
                partDamages = car.PartDamages != null ?
                    car.PartDamages.Select(pd => new {
                        partTypeId = pd.PartTypeId,
                        damageTypeId = pd.DamageTypeId
                    }) :
                    Enumerable.Empty<object>()
            };

            ViewBag.CarData = JsonSerializer.Serialize(carData);
            return View("~/Views/CarDamage/DamageRecording.cshtml");
        }

        [HttpGet("GetDamages/{carId}")]
        public async Task<IActionResult> GetDamages(int carId)
        {
            if (carId <= 0)
            {
                throw new ValidationException("Invalid car id");
            }

            var damages = await _carDamageManager.GetAllDamagedPartsAsync(carId);
            return Json(damages);
        }
    }
}