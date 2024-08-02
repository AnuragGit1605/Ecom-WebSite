using EcomWeb1.Models;
using EcomWeb1.DataAccess.Repo.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomWeb1.DataAccess.Repo.IRepo
{
    public interface IOrderDetailsRepo:IRepo<OrderDetail>
    {
    }
}
