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
    using System.ComponentModel;
    public class CollectionBase<T> :
        System.Collections.ObjectModel.ObservableCollection<T>,
        System.ComponentModel.INotifyPropertyChanged where T : new()
    {
    }
}
