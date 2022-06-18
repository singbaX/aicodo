using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCodo
{
    public interface IView
    {
    }

    public interface ICloseable
    {
        void RequireClose(object state);
    }
}
