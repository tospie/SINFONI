using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public interface IService
    {
        Context Context { get; }

        Delegate this[string name] { set; }
    }
}
