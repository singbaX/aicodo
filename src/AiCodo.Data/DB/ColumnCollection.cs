/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiCodo.Data
{
    public class ColumnCollection : CollectionBase<Column>
    {
        public Column this[string name]
        {
            get
            {
                return Items.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
