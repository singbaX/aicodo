using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo.Data
{
    public class SqlRequest:ISqlRequest
    {
        #region 属性 SqlName
        private string _SqlName = string.Empty;
        public string SqlName
        {
            get
            {
                return _SqlName;
            }
            set
            {
                _SqlName = value;
            }
        }
        #endregion

        #region 属性 Parameters
        private Dictionary<string, object> _Parameters = null;
        public Dictionary<string, object> Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new Dictionary<string, object>();
                }
                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }
        #endregion

        public virtual object[] GetNameValues()
        {
            return Parameters.ToNameValues();
        }
    }
}
