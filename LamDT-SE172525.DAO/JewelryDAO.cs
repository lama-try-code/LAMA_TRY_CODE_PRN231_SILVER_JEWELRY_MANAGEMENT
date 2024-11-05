using LamDT_SE172525.BOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.DAO
{
    public class JewelryDAO
    {
        private SilverJewelry2023DBContext context;
        private static JewelryDAO instance;

        public JewelryDAO()
        {
            context = new SilverJewelry2023DBContext();
        }

        public static JewelryDAO Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new JewelryDAO();
                }
                return instance;
            }
        }

        public async Task<List<SilverJewelry>> GetAll()
        {
            return await context.SilverJewelries.Include(x => x.Category).ToListAsync();
        }

        public async Task<List<SilverJewelry>> SearchJewelryAsync(string? name, decimal? weight)
        {
            IQueryable<SilverJewelry> query = context.Set<SilverJewelry>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(j => EF.Functions.Like(j.SilverJewelryName, $"%{name}%"));
            }

            if (weight.HasValue)
            {
                decimal tolerance = 0.1M;
                query = query.Where(j =>
                    j.MetalWeight >= weight.Value - tolerance &&
                    j.MetalWeight <= weight.Value + tolerance);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> AddJewelry(SilverJewelry jewelry)
        {
            SilverJewelry? check = await this.GetSilverJewelryByName(jewelry.SilverJewelryName);
            if (check != null)
            {
                return false;
            }

            bool categoryExists = await context.Categories
                .AnyAsync(c => c.CategoryId == jewelry.CategoryId);
            if (!categoryExists)
            {
                return false;
            }

            try
            {
                await context.SilverJewelries.AddAsync(jewelry);
                return await context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<SilverJewelry?> GetSilverJewelryByName(string silverJewelryName)
        {
            return await context.SilverJewelries.SingleOrDefaultAsync(x => x.SilverJewelryName.Equals(silverJewelryName));
        }

        public async Task<bool> RemoveJewelry(String id)
        {
            bool result = false;
            SilverJewelry? check = await this.GetSilverJewelryById(id);
            if (check != null)
            {

                try
                {
                    context.SilverJewelries.Remove(check);
                    result = await context.SaveChangesAsync() > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }
        public async Task<bool> UpdateJewelry(SilverJewelry silverJewelry)
        {
            bool result = false;
            SilverJewelry? check = await this.GetSilverJewelryById(silverJewelry.SilverJewelryId);
            if (check != null)
            {

                try
                {
                    context.Entry(check).CurrentValues.SetValues(silverJewelry);
                    result = await context.SaveChangesAsync() > 0 ? true : false;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        public async Task<SilverJewelry?> GetSilverJewelryById(String id)
        {
            return await context.SilverJewelries.Where(x => x.SilverJewelryId.Equals(id)).Include(x => x.Category).SingleOrDefaultAsync();
        }
    }
}
