using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using NewsPortal.Persistence.DTO;
using NewsPortal.Persistence;
using NewsPortal.DesktopApplication.Model;

namespace NewsPortal.DesktopApplication.ViewModel
{
    class ListViewModel : ViewModelBase
    {
        private readonly INewsService _service;
        private ArticleListDTO _articles;
        public event EventHandler<int?> OpenForEdit;
        public ArticleListDTO ArticleList
        {
            get => _articles;
            set
            {
                _articles = value;
                OnPropertyChanged();
            }
        }

        private List<int> _pages;
        public List<int> Pages
        {
            get => _pages;
            set
            {
                _pages = value;
                OnPropertyChanged();
            }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if(value != _currentPage)
                {
                    LoadAsync(value);
                    _currentPage = value;
                    OnPropertyChanged();
                    
                }
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

        public ListViewModel(INewsService service)
        {
            _service = service;
            Pages = new List<int>();
        }

        private async void ButtonPressed(string text, int id)
        {
            Console.WriteLine("%0 - %1", text, id);

            if (text == "EDIT")
            {
                ToEdit(id);
            }
            else if ( text == "HIGHLIGHT")
            {
                await _service.HighlightArticleAsync(id);
                await LoadAsync(_currentPage);
            }
            else if ( text == "PUBLISH")
            {
                await _service.PublishArticleAsync(id);
                await LoadAsync(_currentPage);
            }
            else if (text == "UNPUBLISH")
            {
                await _service.UnPublishArticleAsync(id);
                await LoadAsync(_currentPage);
            }
            else if (text == "DELETE")
            {
                await _service.DeleteArticleAsync(id);
                await LoadAsync(_currentPage);
            }

        }

        public async Task<bool> LoadAsync(int page = 1)
        {
            try
            {
                ArticleListDTO result = await _service.LoadArticlesAsync(page);
                result.Articles = new ObservableCollection<ArticleListElemDTO>(result.Articles);
                ArticleList = result;
                Pages = Enumerable.Range(1, ArticleList.PageCount).ToList();
                CurrentPage = page;
            }
            catch (NetworkException ex)
            {
                OnMessageApplication($"Váratlan hiba történt! ({ex.Message})");
                return false;
            }

            return true;
        }

        private void ToEdit(int? id)
        {
            OpenForEdit(this, id);
        }
    }
}
