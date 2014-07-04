using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.ExecutionEngine
{
    public interface IExecutionInstanceWorker
    {
        bool Process(ExecutionInstance executionInstance);
    }
}
