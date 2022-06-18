/*
 * 此为列表类的查询模型，可应用于DataGrid等，带过滤条件、分页功能
 * SqlName：对应SQL语句的全路径{tablename}.{sqlname}
 * 
 */
namespace AiCodo.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SqlFilterItemsSource : EntityBase
    {
        public event EventHandler ItemsChanged;

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
                RaisePropertyChanged("SqlName");
            }
        }
        #endregion

        #region 属性 Filter
        private ISqlFilter _Filter = null;
        public ISqlFilter Filter
        {
            get
            {
                return _Filter;
            }
            set
            {
                _Filter = value;
                RaisePropertyChanged("Filter");
                RaiseReloadItems();
            }
        }
        #endregion

        #region 属性 Sort
        private Sort _Sort = Sort.SortOfUpdateTimeDesc;
        public Sort Sort
        {
            get
            {
                return _Sort;
            }
            set
            {
                _Sort = value;
                RaisePropertyChanged("Sort");
            }
        }
        #endregion

        #region 属性 Total
        private int _Total = 0;
        public int Total
        {
            get
            {
                return _Total;
            }
            set
            {
                _Total = value;
                RaisePropertyChanged("Total");
            }
        }
        #endregion

        #region 属性 PageIndex
        private int _PageIndex = 1;
        public int PageIndex
        {
            get
            {
                return _PageIndex;
            }
            set
            {
                if (_PageIndex == value)
                {
                    return;
                }

                _PageIndex = value;
                RaisePropertyChanged("PageIndex");
                ResetItems();
            }
        }
        #endregion

        #region 属性 PageSize
        private int _PageSize = 30;
        public int PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                if (_PageSize == value)
                {
                    return;
                }
                _PageSize = value;
                RaisePropertyChanged("PageSize");
                RaiseReloadItems();
            }
        }
        #endregion

        #region 属性 Items
        private IEnumerable _Items = null;
        public IEnumerable Items
        {
            get
            {
                return _Items;
            }
            private set
            {
                _Items = value;
                RaisePropertyChanged("Items");
                ItemsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private IEnumerable<DynamicEntity> GetItems()
        {
            ResetTotalCount();
            if (SqlName.IsNullOrEmpty())
            {
                yield break;
            }

            var data = SqlService.ExecuteSqlQuery<DynamicEntity>(SqlName, Filter, Sort, PageIndex, PageSize);
            if (data == null)
            {
                yield break;
            }
            else
            {
                var count = 0;
                foreach (var item in data)
                {
                    yield return item;
                    count++;
                }
                this.Log($"[{SqlName}]查询数据[{count}]", Category.Debug);
            }
            yield break;
        }

        public void ResetItems()
        {
            Items = GetItems().ToList();
        }
        #endregion

        public IEnumerable<DynamicEntity> Find(Func<DynamicEntity,bool> func)
        {
            if(_Items == null)
            {
                yield break;
            }
            foreach(var item in _Items)
            {
                if(item is DynamicEntity d && func(d))
                {
                    yield return d;
                }
            }
        }

        #region load
        private void RaiseReloadItems()
        {
            if (PageIndex > 1)
            {
                PageIndex = 1;
            }
            else
            {
                ResetItems();
            }
        }

        private void ResetTotalCount()
        {
            if (SqlName.IsNullOrEmpty())
            {
                Total = 0;
                return;
            }

            Total = SqlService.ExecuteSqlCount(SqlName, Filter);
        }
        #endregion
    }
}
