/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
namespace AiCodo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DBEntity : Entity
    {
        public DBEntity()
        {
            IsValid = true;
            CreateUser = 0;
            CreateTime = DateTime.Now;
            UpdateUser = 0;
            UpdateTime = DateTime.Now;
        }

        #region IsValid
        public bool IsValid
        {
            get
            {
                return GetFieldValue<bool>("IsValid", true);
            }
            set
            {
                SetFieldValue("IsValid", value);
            }
        }
        #endregion

        #region CreateUser
        public int CreateUser
        {
            get
            {
                return GetFieldValue<int>("CreateUser", 0);
            }
            set
            {
                SetFieldValue("CreateUser", value);
            }
        }
        #endregion

        #region CreateTime
        public DateTime CreateTime
        {
            get
            {
                return GetFieldValue<DateTime>("CreateTime", DateTime.Now);
            }
            set
            {
                SetFieldValue("CreateTime", value);
            }
        }
        #endregion

        #region UpdateUser
        public int UpdateUser
        {
            get
            {
                return GetFieldValue<int>("UpdateUser", 0);
            }
            set
            {
                SetFieldValue("UpdateUser", value);
            }
        }
        #endregion

        #region UpdateTime
        public DateTime UpdateTime
        {
            get
            {
                return GetFieldValue<DateTime>("UpdateTime", DateTime.Now);
            }
            set
            {
                SetFieldValue("UpdateTime", value);
            }
        }
        #endregion

        public virtual void SetCreateUser(int userId)
        {
            CreateUser = userId;
            UpdateUser = userId;
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
        }

        public virtual void SetUpdateUser(int userId)
        {
            UpdateUser = userId;
            UpdateTime = DateTime.Now;
        }
    }
}
