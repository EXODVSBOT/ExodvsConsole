using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Class;

namespace Aplication.InternalServices
{
    public interface IOperationResultService
    {
        void CreateFile();
        Task<OperationResult> Save(OperationResult operation);
        Task<OperationResult> Get(OperationResult operation, int? take);

    }
}
