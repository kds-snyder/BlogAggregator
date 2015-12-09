using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Domain
{
    public enum BlogTypes
    {
        WordPress=1
    }

    // Application types for external logins
    public enum ApplicationTypes
    {
        JavaScript = 0,
        NativeConfidential = 1
    };
}
