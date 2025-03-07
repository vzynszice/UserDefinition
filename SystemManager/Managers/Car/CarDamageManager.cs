using DAL.Enums;
using DAL.Exceptions;
using DAL.Models;
using DevExtreme.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using SystemManager.Abstractions;
using SystemManager.Abstractions.Car;
using SystemManager.Abstractions.Common;
using SystemManager.Abstractions.Data;

namespace SystemManager.Managers.Car
{
    public class CarDamageManager : BaseManager, ICarDamageManager
    {
        private readonly ICarDamageService _carDamageService;

        public CarDamageManager(
            IRepositoryManager repositoryManager,
            ICarDamageService carDamageService,
            ILogger<CarDamageManager> logger,
            ICurrentUserService currentUserService)
            : base(repositoryManager, logger, currentUserService)
        {
            _carDamageService = carDamageService;
        }

        public async Task<object> GetCarDamagesForGrid(DataSourceLoadOptions loadOptions)
        {
            await LogActionAsync("Get Car Damages Grid");
            return await _carDamageService.GetCarDamagesForGrid(loadOptions);
        }

        public async Task<CarModel> GetCarDamagesAsync(int carId)
        {
            await LogActionAsync("Get Car Damages", $"CarId: {carId}");
            var car = await _carDamageService.GetCarDamagesAsync(carId);

            if (car == null)
            {
                throw new NotFoundException($"Car with ID {carId} not found");
            }

            return car;
        }

        public async Task<IEnumerable<CarModel>> GetCarsByPlateAsync(string plateNumber)
        {
            await LogActionAsync("Get Cars By Plate", $"Plate: {plateNumber}");

            if (string.IsNullOrEmpty(plateNumber))
            {
                throw new ValidationException("Plate number cannot be empty");
            }

            return await _carDamageService.GetCarsByPlateAsync(plateNumber);
        }

        public async Task<IEnumerable<CarModel>> GetCarsByDamageTypeAsync(DamageType damageType)
        {
            await LogActionAsync("Get Cars By Damage Type", $"DamageType: {damageType}");

            if (!Enum.IsDefined(typeof(DamageType), damageType))
            {
                throw new ValidationException($"Invalid damage type: {damageType}");
            }

            return await _carDamageService.GetCarsByDamageTypeAsync(damageType);
        }

        public async Task UpdateCarDamagesAsync(int carId, IEnumerable<PartDamage> damages)
        {
            await LogActionAsync("Update Car Damages", $"CarId: {carId}");

            if (damages == null || !damages.Any())
            {
                throw new ValidationException("Damage list cannot be empty");
            }

            foreach (var damage in damages)
            {
                if (!Enum.IsDefined(typeof(PartType), damage.PartTypeId))
                {
                    throw new ValidationException($"Invalid part type: {damage.PartTypeId}");
                }

                if (!Enum.IsDefined(typeof(DamageType), damage.DamageTypeId))
                {
                    throw new ValidationException($"Invalid damage type: {damage.DamageTypeId}");
                }
            }

            await _carDamageService.UpdateCarDamagesAsync(carId, damages);
        }

        public async Task<List<DamageType>> GetDamagesByPartTypeAsync(int carId, PartType partType)
        {
            await LogActionAsync("Get Damages By Part Type", $"CarId: {carId}, PartType: {partType}");

            if (!Enum.IsDefined(typeof(PartType), partType))
            {
                throw new ValidationException($"Invalid part type: {partType}");
            }

            return await _carDamageService.GetDamagesByPartTypeAsync(carId, partType);
        }

        public async Task<List<PartDamage>> GetAllDamagedPartsAsync(int carId)
        {
            await LogActionAsync("Get All Damaged Parts", $"CarId: {carId}");
            return await _carDamageService.GetAllDamagedPartsAsync(carId);
        }

        public async Task<List<DamageType>> GetDamageTypes()
        {
            await LogActionAsync("Get Damage Types");
            return await _carDamageService.GetDamageTypes();
        }

        public async Task<List<PartType>> GetPartTypes()
        {
            await LogActionAsync("Get Part Types");
            return await _carDamageService.GetPartTypes();
        }

        public async Task<CarModel> GetByIdAsync(int id)
        {
            await LogActionAsync("Get Car By Id", $"CarId: {id}");
            var car = await _carDamageService.GetByIdAsync(id);

            if (car == null)
            {
                throw new NotFoundException($"Car with ID {id} not found");
            }

            return car;
        }

        public async Task CreateAsync(CarModel car)
        {
            await LogActionAsync("Create Car", $"Plate: {car.Plate}");

            if (car == null)
            {
                throw new ValidationException("Car cannot be null");
            }

            if (string.IsNullOrEmpty(car.Plate))
            {
                throw new ValidationException("Car plate is required");
            }

            await _carDamageService.CreateAsync(car);
        }

        public async Task UpdateAsync(CarModel car)
        {
            await LogActionAsync("Update Car", $"CarId: {car.id}");

            if (car == null)
            {
                throw new ValidationException("Car cannot be null");
            }

            var existingCar = await GetByIdAsync(car.id);
            await _carDamageService.UpdateAsync(car);
        }

        public async Task DeleteAsync(int id)
        {
            await LogActionAsync("Delete Car", $"CarId: {id}");

            var car = await GetByIdAsync(id);
            await _carDamageService.DeleteAsync(id);
        }
    }
}