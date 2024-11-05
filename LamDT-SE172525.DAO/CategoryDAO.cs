using LamDT_SE172525.BOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.DAO
{
    public class CategoryDAO
    {
        private SilverJewelry2023DBContext context;
        private static CategoryDAO instance;

        public CategoryDAO()
        {
            context = new SilverJewelry2023DBContext();
        }

        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CategoryDAO();
                }
                return instance;
            }
        }

        public async Task<List<Category>> GetAll()
        {
            return await context.Categories.ToListAsync(); 
        }

        public async Task<Category?> GetById(String id)
        {
            return await context.Categories.SingleOrDefaultAsync(x => x.CategoryId.Equals(id));
        }
    }
}
