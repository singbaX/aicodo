using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo.Data
{
    #region update
    public class UpdateDefinition : Entity
    {
        public UpdateDefinition()
        {

        }

        public UpdateDefinition(object[] nameValues)
        {
            if (nameValues != null && nameValues.Length > 0)
            {
                for (int i = 0; i < nameValues.Length - 1; i += 2)
                {
                    this.SetValue(nameValues[i].ToString(), nameValues[i + 1]);
                }
            }
        }

        public UpdateDefinition(IEntity entity)
        {
            if (entity != null)
            {
                var nameValues = entity.GetNameValues();
                for (int i = 0; i < nameValues.Length - 1; i += 2)
                {
                    this.SetValue(nameValues[i].ToString(), nameValues[i + 1]);
                }
            }
        }

        public virtual string CreateUpdateFields(IEntity parameters)
        {
            StringBuilder sb = new StringBuilder();
            var index = 0;
            foreach (var name in this.Keys)
            {
                var pname = $"{name}";
                var pvalue = GetValue(name);
                parameters.SetValue(pname, pvalue);
                if (index == 0)
                {
                    sb.Append($"{name}=@{pname}");
                }
                else
                {
                    sb.Append($",\r\n{name}=@{pname}");
                }
                index++;
            }
            return sb.ToString();
        }

        public virtual string CreateUpdateFields(IEntity parameters, string parameterPrefix, ref int parameterIndex)
        {
            StringBuilder sb = new StringBuilder();
            var index = 0;
            foreach (var name in this.Keys)
            {
                parameterIndex++;
                var pname = $"@{parameterPrefix}{parameterIndex}";
                var pvalue = GetValue(name);
                parameters.SetValue(pname, pvalue);
                if (index == 0)
                {
                    sb.Append($"{name}={pname}");
                }
                else
                {
                    sb.Append($",\r\n{name}={pname}");
                }
                index++;
            }
            return sb.ToString();
        }

        public UpdateDefinition Set(string name, object value)
        {
            this.SetValue(name, value);
            return this;
        }
    }
    #endregion
}
