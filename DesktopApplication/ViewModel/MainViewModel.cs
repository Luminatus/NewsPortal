using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.DesktopApplication.Model;
using NewsPortal.Persistence;
using NewsPortal.Persistence.DTO;
using System.Collections.ObjectModel;


namespace NewsPortal.DesktopApplication.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private Boolean _isEdit;

        private ArticleListDTO _articles;
        private ArticleDTO _currentArticle;
        private readonly INewsService _service;
        public event EventHandler ExitApplication;


        public Boolean IsEdit
        {
            get => _isEdit;
            set
            {
                _isEdit = value;
                OnPropertyChanged();
            }

        }
        public ArticleListDTO ArticleList
        {
            get => _articles;
            set
            {
                _articles = value;
                OnPropertyChanged();
            }
        }

        public ArticleDTO CurrentArticle
        {
            get => _currentArticle;
            set
            {
                _currentArticle = value;
                OnPropertyChanged();
            }
        }

        private DelegateCommand _listButton;
        public DelegateCommand ListButton
        {
            get
            {
                return _listButton ?? (_listButton = new DelegateCommand((param) => { ToList(); }));
            }
        }

        private DelegateCommand _buttonPress;
        public DelegateCommand ButtonPress
        {
            get
            {
                return _buttonPress ?? (_buttonPress = new DelegateCommand((param) => {
                    var paramT = (Tuple<String, int>)param;
                    if (paramT == null)
                        return;
                    var text = paramT.Item1;
                    var id = paramT.Item2;
                    ButtonPressed(text, id);
                }));
            }
        }
        
        public MainViewModel(INewsService service)
        {
            _service = service;
        }

        public async void OnLoaded(object sender, EventArgs e)
        {
            ToList();
        }


        public async void LoadAsync()
        {
            try
            {
                ArticleList = await _service.LoadArticlesAsync().ConfigureAwait(false);
            }
            catch (NetworkException ex)
            {
                OnMessageApplication($"Váratlan hiba történt! ({ex.Message})");
            }

            IsEdit = false;
        }

        
        private void ButtonPressed(string text, int id)
        {
            Console.WriteLine("%0 - %1", text, id);

            if(text == "EDIT")
            {
                ToEdit(id);
            }

        }

        private async void ToList()
        {
            ArticleListDTO result = await _service.LoadArticlesAsync();
            result.Articles = new ObservableCollection<ArticleListElemDTO>(result.Articles);
            ArticleList = result;
            IsEdit = false;
        }

        private async void ToEdit(int? id = null)
        {
            if (id.HasValue)
            {
                CurrentArticle = await _service.GetArticleAsync(id.Value);
            }
                
            else
            {
                CurrentArticle = new ArticleDTO();
            }                


            IsEdit = true;

        }
        
    }
}
