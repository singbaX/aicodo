using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AiCodo
{
    public interface IContentLoader
    {
        Task<object> LoadContentAsync(string source);
    }
}
