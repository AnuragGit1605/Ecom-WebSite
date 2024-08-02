using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomWeb1.DataAccess.Repo.IRepo
{
    public interface IUnitOfWork
    {
        ICategoryRepo Category { get; }
        ICoverTypeRepo CoverType { get; }
        IProductRepo Product { get; }
        ICompanyRepo Company { get; }
        IApplicationUserRepo ApplicationUser { get; }
        IOrderDetailsRepo OrderDetails { get; }
        IOrderHeaderRepo OrderHeader { get; }
        IShoppingKartRepo ShoppingKart { get; }
        void save();
    }
}
