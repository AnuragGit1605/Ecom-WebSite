using EcomWeb1.Data;
using EcomWeb1.DataAccess.Repo.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomWeb1.DataAccess.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepo(_context);
            CoverType = new CoverTypeRepo(_context);
            Product = new ProductRepo(_context);
            Company = new CompanyRepo(_context);
            ApplicationUser = new ApplicationUserRepo(_context);
            OrderDetails = new OrderDetailRepo(_context);
            OrderHeader = new OrderHeaderRepo(_context);
            ShoppingKart = new ShoppingKartRepo(_context);
           
        }

        public ICategoryRepo Category { private set; get; }

        public ICoverTypeRepo CoverType { private set; get; }
        public IProductRepo Product { private set; get; }
        public ICompanyRepo Company { private set; get; }
        public IApplicationUserRepo ApplicationUser { private set; get; }
        public IOrderHeaderRepo OrderHeader { private set; get; }
        public IOrderDetailsRepo OrderDetails { private set; get; }
        public IShoppingKartRepo ShoppingKart { private set; get; }
        public void save()
        {
            _context.SaveChanges();
        }
    }
}
