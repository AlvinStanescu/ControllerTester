using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.Environment
{
    public interface IConfigurationControl
    {
        void Save();
        bool Validate();
    }
}
