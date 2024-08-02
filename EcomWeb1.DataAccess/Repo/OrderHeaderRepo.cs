using EcomWeb1.Data;
using EcomWeb1.DataAccess.Repo.IRepo;
using EcomWeb1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomWeb1.DataAccess.Repo
{
    public class OrderHeaderRepo : Repo<OrderHeader>, IOrderHeaderRepo
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
