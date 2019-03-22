using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaykiContract.Enums
{
    /// <summary>
    /// 脚本应用账户操作类型定义
    /// </summary>
    enum APP_OPERATOR_TYPE
    {
        /// <summary>
        /// 自由账户加
        /// </summary>
        ENUM_ADD_FREE_OP = 1,      //
        /// <summary>
        /// 自由账户减
        /// </summary>
        ENUM_SUB_FREE_OP = 2,      //
        /// <summary>
        /// 冻结账户加
        /// </summary>
        ENUM_ADD_FREEZED_OP = 3,   //
        /// <summary>
        /// 冻结账户减
        /// </summary>
        ENUM_SUB_FREEZED_OP = 4     //
    }
}
