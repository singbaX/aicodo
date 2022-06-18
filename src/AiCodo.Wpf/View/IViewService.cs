using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCodo
{
    public interface IViewService
    {
        Task<IView> CreateView(string name, Dictionary<string, object> args);

        void SetContainer(IContainer container);

        void ShowDialog(string dialogName, Dictionary<string, object> args);
    }
}
