using DAL.Enums;
using DAL.Models;
using DevExtreme.AspNet.Mvc;

namespace SystemManager.Abstractions.Car
{
    public interface ICarDamageManager
    {
        Task<object> GetCarDamagesForGrid(DataSourceLoadOptions loadOptions);
        Task<CarModel> GetCarDamagesAsync(int carId);
        Task<IEnumerable<CarModel>> GetCarsByDamageTypeAsync(DamageType damageType);
        Task UpdateCarDamagesAsync(int carId, IEnumerable<PartDamage> damages);
        Task<List<DamageType>> GetDamagesByPartTypeAsync(int carId, PartType partType);
        Task<List<PartDamage>> GetAllDamagedPartsAsync(int carId);
        Task<List<DamageType>> GetDamageTypes();
        Task<List<PartType>> GetPartTypes();
        Task<CarModel> GetByIdAsync(int id);
        Task CreateAsync(CarModel car);
        Task UpdateAsync(CarModel car);
        Task DeleteAsync(int id);
        Task<IEnumerable<CarModel>> GetCarsByPlateAsync(string plateNumber);
    }
}
