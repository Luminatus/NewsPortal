using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.DesktopApplication.Model;
using NewsPortal.Persistence;
using NewsPortal.Persistence.DTO;
using System.Collections.ObjectModel;
using System.Net.Http.Handlers;
using System.Windows;

namespace NewsPortal.DesktopApplication.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private Boolean _isEdit;
     
        private readonly INewsService _service;
        public event EventHandler LoggedOut;

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        private ListViewModel _listVM;
        public ListViewModel ListVM { get
            {
                return _listVM;
            }
        }

        private EditViewModel _editVM;
        public EditViewModel EditVM
        {
            get
            {
                return _editVM;
            }
        }

        //public ViewModelBase ActiveViewModel;


        public Boolean IsEdit
        {
            get => _isEdit;
            set
            {
                _isEdit = value;
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
        
        private DelegateCommand _newArticleButton;
        public DelegateCommand NewArticleButton
        {
            get
            {
                return _newArticleButton ?? (_newArticleButton = new DelegateCommand((param) => { ToEdit(); }));
            }
        }

        private DelegateCommand _signOutButton;
        public DelegateCommand SignOutButton
        {
            get
            {
                return _signOutButton ?? (_signOutButton = new DelegateCommand( (param) => { SignOut(); }));
            }
        }

        public async Task SignOut()
        {
            await _service.LogoutAsync();
            LoggedOut.Invoke(this,null);
        }


        public MainViewModel(INewsService service)
        {
            _service = service;
            _listVM = new ListViewModel(_service);
            _editVM = new EditViewModel(_service);
            _listVM.OpenForEdit += new EventHandler<int?>((obj,p) => ToEdit(p));
            _editVM.BackToList += new EventHandler((obj, p) => ToList());
            _listVM.MessageApplication += new EventHandler<MessageEventArgs>((obj, msgArg) => { OnMessageApplication(obj, msgArg.Message); });
            _editVM.MessageApplication += new EventHandler<MessageEventArgs>((obj, msgArg) => { OnMessageApplication(obj, msgArg.Message); });
        }

        public async void OnLoaded(object sender, EventArgs e)
        {
            ToList();
            GetUser();
        }
        
        private async void ToList()
        {
            bool success = await ListVM.LoadAsync(1);
            if(success)
            {
                IsEdit = false;
            }
            
        }

        private async void ToEdit(int? id = null)
        {
            bool success = await _editVM.LoadAsync(id);            
            if(success)
            {
                IsEdit = true;
            }
        }

        private async void GetUser()
        {
            var user = await _service.GetUserAsync();
            UserName = user.Name;
        }
        
    }
}
