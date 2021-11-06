using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo.Data
{
    public interface ISqlRequest
    {
        string SqlName { get; set; }

        object[] GetNameValues();
    }
}
