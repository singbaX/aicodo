namespace AiCodo.WpfApp
{
    public class MainViewModel : ViewModelBase
    {
        public static MainViewModel Current { get; private set; } = new MainViewModel();

        MainViewModel()
        {
        }

        #region 属性 Modules
        private IEnumerable<DynamicEntity> _Modules = null;
        public IEnumerable<DynamicEntity> Modules
        {
            get
            {
                if (_Modules == null)
                {
                    _Modules = new List<DynamicEntity>
                    {
                        new DynamicEntity("Name","首页","Url","HomePage"), 
                    };
                }
                return _Modules;
            }
        }
        #endregion

        #region ShowPageCommand
        private RelayCommand _ShowPageCommand = null;
        public RelayCommand ShowPageCommand
        {
            get
            {
                if (_ShowPageCommand == null)
                {
                    _ShowPageCommand = new RelayCommand(OnShowPage, CanShowPage);
                }
                return _ShowPageCommand;
            }
        }

        private bool CanShowPage(object parameter)
        {
            return parameter != null;
        }

        private void OnShowPage(object parameter)
        {
            var url = parameter == null ? "" : parameter.ToString();
            if (url.IsNullOrEmpty())
            {
                url = "Services";
            }
            PageUrl = url;
        }
        #endregion

        #region 属性 PageUrl
        private string _PageUrl = "DataConfig";
        public string PageUrl
        {
            get
            {
                return _PageUrl;
            }
            set
            {
                _PageUrl = value;
                RaisePropertyChanged("PageUrl");
            }
        }
        #endregion
    }
}
