// *************************************************************************************************
// 代码文件：FunctionFlow.cs
// 
// 配置文件：FunctionFlow.xml
// 
// 作    者：SingbaX
// 
// 创建日期：202206
// 
// 功能说明：流程配置文件
//
// 修改记录：
// *************************************************************************************************
namespace AiCodo.Flow.Configs
{
    public interface IFunctionResult
    {
        string ErrorCode { get;  }
        string ErrorMessage { get; }

        bool TryGetValue(string name, out object value);
        void SetValue(string name, object value);
    }
}