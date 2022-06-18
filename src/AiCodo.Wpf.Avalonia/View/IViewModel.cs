using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCodo
{
    public interface IViewModel
    {
        IView View { get; set; }
        void OnOpened(Dictionary<string,object> args);
    }
}
