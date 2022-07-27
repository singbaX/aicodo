// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class DataErrorAsyncValidate : DataErrorValidate
    {
        public Action<Action<bool>> AsyncAction { get; set; }

        public DataErrorAsyncValidate(Action<Action<bool>> action, string error)
        {
            AsyncAction = action;
            Error = error;
        }
    }

    public class DataErrorValidate
    {
        public DataErrorValidate()
        {
        }

        public DataErrorValidate(Func<bool> funcIsValidate, string error)
        {
            FuncIsValidate = funcIsValidate;
            Error = error;
        }

        public Func<bool> FuncIsValidate { get; set; }

        public string Error { get; set; }
    }
}
