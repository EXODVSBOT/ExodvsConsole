using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Record;

namespace Animation.Configuration
{
    public interface IConfigurationAnimation
    {
        ConfigurationResultRecord GetConfiguration();
    }
}
