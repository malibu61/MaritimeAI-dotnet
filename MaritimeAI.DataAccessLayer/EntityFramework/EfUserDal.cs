using MaritimeAI.DataAccessLayer.Abtstract;
using MaritimeAI.DataAccessLayer.Context;
using MaritimeAI.DataAccessLayer.Entities;
using MaritimeAI.DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaritimeAI.DataAccessLayer.EntityFramework
{
    public class EfUserDal : GenericRepository<User>, IUserDal
    {
        public EfUserDal(MaritimeAIContext context) : base(context)
        {
        }
    }
}
