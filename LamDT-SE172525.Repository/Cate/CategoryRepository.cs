using LamDT_SE172525.BOs;
using LamDT_SE172525.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.Repository.Cate
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<List<Category>> GetAllCategories() => await CategoryDAO.Instance.GetAll();

        public async Task<Category?> GetCategory(string id) => await CategoryDAO.Instance.GetById(id);
    }
}
