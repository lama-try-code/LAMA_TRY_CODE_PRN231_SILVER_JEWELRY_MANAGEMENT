using LamDT_SE172525.BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.Repository.Cate
{
    public interface ICategoryRepository
    {
        public Task<Category?> GetCategory(String id);
        public Task<List<Category>> GetAllCategories();
    }
}
