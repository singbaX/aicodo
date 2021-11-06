using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo.Data
{
    public interface IUser : IEntity
    {
        string UserID { get; set; }
        string UserName { get; set; }
    }
}
