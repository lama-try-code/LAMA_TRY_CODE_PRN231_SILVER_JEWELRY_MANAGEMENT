using LamDT_SE172525.BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.Repository.Jewelry
{
    public interface IJewelryRepository
    {
        public Task<List<SilverJewelry>> GetAll();
        public Task<bool> AddJewelry(SilverJewelry jewelry);
        public Task<bool> RemoveJewelry(String id);
        public Task<bool> UpdateJewelry(SilverJewelry silverJewelry);
        public Task<SilverJewelry?> GetSilverJewelryById(String id);
        public Task<List<SilverJewelry>> SearchJewelryAsync(string? name, decimal? weight);

    }
}
