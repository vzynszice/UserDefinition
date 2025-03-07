using DAL.Models;
using DAL.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemManager.Abstractions.Car;
using DAL.Enums;

namespace DevExtremeAspNetCoreApp1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarDamageController : ControllerBase
    {
        private readonly ICarDamageManager _carDamageManager;
        private readonly ILogger<CarDamageController> _logger;

        public CarDamageController(
            ICarDamageManager carDamageManager,
            ILogger<CarDamageController> logger)
        {
            _carDamageManager = carDamageManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("DamageRecording")]
        public IActionResult DamageRecording()
        {
            return Ok("Damage Recording Page");
        }

        [HttpGet("GetCarDamages/{carId}")]
        public async Task<IActionResult> GetCarDamages(int carId)
        {
            if (carId <= 0)
                throw new ValidationException("Invalid car id");

            var car = await _carDamageManager.GetCarDamagesAsync(carId);
            return Ok(car);
        }

        [HttpPost("AddCarDamage")]
        public async Task<IActionResult> AddCarDamage([FromBody] CarModel car)
        {
            if (car == null)
                throw new ValidationException("No car data received");

            if (string.IsNullOrEmpty(car.Plate))
                throw new ValidationException("Car plate is required");

            await _carDamageManager.CreateAsync(car);
            return Ok(car);
        }

        [HttpPut("UpdateCarDamage/{carId}")]
        public async Task<IActionResult> UpdateCarDamage(int carId, [FromBody] IEnumerable<PartDamage> damages)
        {
            if (carId <= 0)
                throw new ValidationException("Invalid car id");

            if (damages == null)
                throw new ValidationException("No damage data received");

            await _carDamageManager.UpdateCarDamagesAsync(carId, damages);
            var updatedCar = await _carDamageManager.GetCarDamagesAsync(carId);
            return Ok(updatedCar);
        }

        [HttpGet("GetDamagesForPart/{carId}/{partType}")]
        public async Task<IActionResult> GetDamagesForPart(int carId, PartType partType)
        {
            if (carId <= 0)
                throw new ValidationException("Invalid car id");

            var damages = await _carDamageManager.GetDamagesByPartTypeAsync(carId, partType);
            return Ok(damages);
        }

        [HttpGet("GetAllDamagedParts/{carId}")]
        public async Task<IActionResult> GetAllDamagedParts(int carId)
        {
            if (carId <= 0)
                throw new ValidationException("Invalid car id");

            var damagedParts = await _carDamageManager.GetAllDamagedPartsAsync(carId);
            return Ok(damagedParts);
        }

        [HttpGet("GetDamageTypes")]
        public async Task<IActionResult> GetDamageTypes()
        {
            var result = await _carDamageManager.GetDamageTypes();
            return Ok(result);
        }

        [HttpGet("GetPartTypes")]
        public async Task<IActionResult> GetPartTypes()
        {
            var result = await _carDamageManager.GetPartTypes();
            return Ok(result);
        }

        [HttpPost("SaveDamages/{carId}")]
        public async Task<IActionResult> SaveDamages(int carId, [FromBody] List<PartDamage> damages)
        {
            if (carId <= 0)
                throw new ValidationException("Invalid car id");

            if (damages == null || !damages.Any())
                throw new ValidationException("No damage data received");

            await _carDamageManager.UpdateCarDamagesAsync(carId, damages);
            return Ok(new { message = "Changes saved successfully" });
        }
    }
}