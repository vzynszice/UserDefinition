
using DAL.Enums;
using DAL.Models;
using DAL.Repositories;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemManager.Abstractions;
using SystemManager.Abstractions.Car;
using SystemManager.Abstractions.Data;


namespace BLL.Services
{
    // BLL/Services/CarDamageService.cs
    public class CarDamageService : GenericService<CarModel>, ICarDamageService
    {
        private readonly IRepositoryManager _repositoryManager;

        // Constructor now accepts IRepositoryManager
        public CarDamageService(IRepositoryManager repositoryManager) : base(repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public override IGenericRepository<CarModel> GetRepository()
        {
            return _repositoryManager.GetRepository<CarModel>();
        }

        public async Task<object> GetCarDamagesForGrid(DataSourceLoadOptions loadOptions)
        {
            var query = GetRepository().GetQueryable()
                .Include(car => car.PartDamages)
                .Select(car => new
                {
                    car.id,
                    car.Plate,
                    PartDamages = car.PartDamages.Select(pd => new
                    {
                        PartType = pd.PartTypeId,
                        DamageType = pd.DamageTypeId
                    }).ToList()
                });

            return await DataSourceLoader.LoadAsync(query, loadOptions);
        }

        public async Task<CarModel> GetCarDamagesAsync(int carId)
        {
            return await GetRepository().GetQueryable()
                .Include(c => c.PartDamages)
                .FirstOrDefaultAsync(c => c.id == carId);
        }

        public async Task<IEnumerable<CarModel>> GetCarsByDamageTypeAsync(DamageType damageType)
        {
            return await GetRepository().GetQueryable()
                .Include(car => car.PartDamages)
                .Where(car => car.PartDamages.Any(pd => pd.DamageTypeId == (int)damageType))
                .ToListAsync();
        }

        public async Task UpdateCarDamagesAsync(int carId, IEnumerable<PartDamage> damages)
        {
            var car = await GetByIdAsync(carId);

            if (car.PartDamages == null)
                car.PartDamages = new List<PartDamage>();
            else
                car.PartDamages.Clear();

            foreach (var damage in damages)
            {
                car.PartDamages.Add(new PartDamage
                {
                    PartTypeId = damage.PartTypeId,
                    DamageTypeId = damage.DamageTypeId
                });
            }

            await UpdateAsync(car);
        }

        public async Task<List<DamageType>> GetDamagesByPartTypeAsync(int carId, PartType partType)
        {
            var car = await GetByIdAsync(carId);

            if (car?.PartDamages == null)
                return new List<DamageType> { DamageType.Original };

            return car.PartDamages
                .Where(pd => pd.PartTypeId == (int)partType)
                .Select(pd => (DamageType)pd.DamageTypeId)
                .DefaultIfEmpty(DamageType.Original)
                .ToList();
        }

        public async Task<List<PartDamage>> GetAllDamagedPartsAsync(int carId)
        {
            var car = await GetByIdAsync(carId);

            if (car?.PartDamages == null)
                return new List<PartDamage>();

            return car.PartDamages
                .Where(pd => pd.DamageTypeId != (int)DamageType.Original)
                .ToList();
        }

        public Task<List<DamageType>> GetDamageTypes()
        {
            return Task.FromResult(Enum.GetValues<DamageType>().ToList());
        }

        public Task<List<PartType>> GetPartTypes()
        {
            return Task.FromResult(Enum.GetValues<PartType>().ToList());
        }

        public async Task<IEnumerable<CarModel>> GetCarsByPlateAsync(string plateNumber)
        {
            return await GetRepository().GetQueryable()
                .Where(car => car.Plate == plateNumber)
                .ToListAsync();
        }
    }
}