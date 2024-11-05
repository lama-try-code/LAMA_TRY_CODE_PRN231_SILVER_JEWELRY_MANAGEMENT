using LamDT_SE172525.BOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LamDT_SE172525.DAO
{
    public class AccountsDAO
    {
        private SilverJewelry2023DBContext context;
        private static AccountsDAO instance;

        public AccountsDAO()
        {
            context = new SilverJewelry2023DBContext();
        }

        public static AccountsDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountsDAO();
                }
                return instance;
            }
        }

        public async Task<BranchAccount?> GetBranchAccount(String email, String password)
        {
            return await context.BranchAccounts.SingleOrDefaultAsync(x => x.AccountPassword.Equals(password) && x.EmailAddress.Equals(email));
        }
    }
}
