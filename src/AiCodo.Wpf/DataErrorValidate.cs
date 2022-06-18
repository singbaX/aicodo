// Copyright (c) 2021 Singba AiCodo.com Corporation. All Rights Reserved.
// Licensed under the MIT License.

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
