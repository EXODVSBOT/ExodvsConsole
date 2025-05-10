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
    public class OperationResultService : IOperationResultService
    {
        private readonly IOperation<OperationResult> _operation;

        public OperationResultService(IOperation<OperationResult> operation)
        {
            _operation = operation;
        }

        public void CreateFile()
        {
            _operation.EnsureFileExists();
        }

        public async Task<OperationResult> Save(OperationResult operation)
        {
            _operation.Create(operation);
            return operation;
        }

        public async Task<OperationResult> Get(OperationResult operation, int? take)
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
