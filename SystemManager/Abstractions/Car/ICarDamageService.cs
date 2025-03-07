using DAL.Enums;
using DAL.Models;
using DevExtreme.AspNet.Mvc;
using SystemManager.Abstractions.Common;

namespace SystemManager.Abstractions.Car
{
    // SystemManager/Abstractions/Car/ICarDamageService.cs
    public interface ICarDamageService : IGenericService<CarModel>
    {
        Task<object> GetCarDamagesForGrid(DataSourceLoadOptions loadOptions);
        Task<CarModel> GetCarDamagesAsync(int carId);
        Task<IEnumerable<CarModel>> GetCarsByDamageTypeAsync(DamageType damageType);
        Task UpdateCarDamagesAsync(int carId, IEnumerable<PartDamage> damages);
        Task<List<DamageType>> GetDamagesByPartTypeAsync(int carId, PartType partType);
        Task<List<PartDamage>> GetAllDamagedPartsAsync(int carId);
        Task<List<DamageType>> GetDamageTypes();
        Task<List<PartType>> GetPartTypes();
        Task<IEnumerable<CarModel>> GetCarsByPlateAsync(string plateNumber);
    }
}
