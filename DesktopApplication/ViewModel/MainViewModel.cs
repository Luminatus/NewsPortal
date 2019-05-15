using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.DesktopApplication.Model;
using NewsPortal.Persistence;
using System.Collections.ObjectModel;

namespace NewsPortal.DesktopApplication.ViewModel
{
    class MainViewModel : ViewModelBase
    {

        private ObservableCollection<Article> _articles;
        private readonly INewsService _service;
        public event EventHandler ExitApplication;

        public ObservableCollection<Article> Articles
        {
            get => _articles;
            set
            {
                _articles = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(INewsService service)
        {
            _service = service;
        }

        public async void OnLoaded(object sender, EventArgs e)
        {
            try
            {
                var ret = new ObservableCollection<String>(await _service.Test().ConfigureAwait(false));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Something not right");
            }
            int col = 12;
        }


        public async void LoadAsync()
        {
            try
            {
                Articles = new ObservableCollection<Article>(await _service.LoadArticlesAsync().ConfigureAwait(false));
            }
            catch (NetworkException ex)
            {
                OnMessageApplication($"Váratlan hiba történt! ({ex.Message})");
            }
        }
    }
}
