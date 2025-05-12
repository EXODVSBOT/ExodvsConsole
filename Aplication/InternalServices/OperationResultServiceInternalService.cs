using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Class;
using Domain.Record;
using TxtDatabase;

namespace Aplication.InternalServices
{
    public class OperationResultServiceInternalService : IOperationResultInternalService
    {
        private readonly IOperationRepository<OperationResultDomain> _operation;

        public OperationResultServiceInternalService(IOperationRepository<OperationResultDomain> operation)
        {
            _operation = operation;
        }

        public void CreateFile()
        {
            _operation.EnsureFileExists();
        }

        public async Task<OperationResultDomain> Save(OperationResultDomain operation)
        {
            _operation.Create(operation);
            return operation;
        }

        public async Task<OperationResultDomain> Get(OperationResultDomain operation, int? take)
        {
            var result = _operation.ReadAll()
                .OrderByDescending(x => x.OperationDate)
                .ToList();

            if (take > 0)
                result.Take((int)take);

            return operation;
        }



    }
}
