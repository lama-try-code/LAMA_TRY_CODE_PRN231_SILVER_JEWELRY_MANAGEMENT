using LamDT_SE172525.BOs;
using LamDT_SE172525.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.Repository.Jewelry
{
    public class JewelryRepository : IJewelryRepository
    {
        public async Task<bool> AddJewelry(SilverJewelry jewelry) => await JewelryDAO.Instance.AddJewelry(jewelry);

        public async Task<List<SilverJewelry>> GetAll() => await JewelryDAO.Instance.GetAll();

        public async Task<SilverJewelry?> GetSilverJewelryById(string id) => await JewelryDAO.Instance.GetSilverJewelryById(id);

        public async Task<bool> RemoveJewelry(string id) => await JewelryDAO.Instance.RemoveJewelry(id);

        public async Task<bool> UpdateJewelry(SilverJewelry silverJewelry) => await JewelryDAO.Instance.UpdateJewelry(silverJewelry);

        public async Task<List<SilverJewelry>> SearchJewelryAsync(string? name, decimal? weight) => await JewelryDAO.Instance.SearchJewelryAsync(name, weight);
    }
}
