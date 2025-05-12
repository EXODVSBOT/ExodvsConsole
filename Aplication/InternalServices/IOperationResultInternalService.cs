using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Class;

namespace Aplication.InternalServices
{
    public interface IOperationResultInternalService
    {
        void CreateFile();
        Task<OperationResultDomain> Save(OperationResultDomain operation);
        Task<OperationResultDomain> Get(OperationResultDomain operation, int? take);

    }
}
