using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Record;

namespace Aplication.InternalServices
{
    public interface IVerifyConditionsToRunInternalService
    {
        Task<ConditionsToRunRecord> StartVerification(ConfigurationResultRecord configuration);
    }
}
