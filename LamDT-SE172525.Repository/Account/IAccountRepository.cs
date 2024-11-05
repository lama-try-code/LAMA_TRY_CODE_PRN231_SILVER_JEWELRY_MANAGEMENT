using LamDT_SE172525.BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.Repository.Account
{
    public interface IAccountRepository
    {
        public Task<String> GetBranchAccount(String email, String password);
    }
}
