using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.Persistence;
using NewsPortal.Persistence.DTO;
using NewsPortal.DesktopApplication.Model;
using Microsoft.Win32;
using System.IO;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace NewsPortal.DesktopApplication.ViewModel
{
    class EditViewModel : ViewModelBase
    {
        private readonly INewsService _service;
        public event EventHandler BackToList;
        private ArticleClientData _currentArticle;
        public ArticleClientData CurrentArticle
        {
            get => _currentArticle;
            set
            {
                _currentArticle = value;
                OnPropertyChanged();
            }
        }

        private DelegateCommand _openImageUpload;
        public DelegateCommand OpenImageUpload
        {
            get
            {
                return _openImageUpload ?? (_openImageUpload = new DelegateCommand((param) => OpenImage()));
            }
        }


        private DelegateCommand _deleteImageButton;
        public DelegateCommand DeleteImageButton
        {
            get
            {
                return _deleteImageButton ?? (_deleteImageButton = new DelegateCommand((param) => {
                    if(param is ImageClientData)
                    {
                        ImageClientData data = param as ImageClientData;
                        data.IsDeleted = true;

                        CollectionViewSource.GetDefaultView(CurrentArticle.Images).Refresh();
                    }

                        Console.WriteLine("szarügy");
                }));
            }
        }

        private DelegateCommand _recoverImageButton;
        public DelegateCommand RecoverImageButton
        {
            get
            {
                return _recoverImageButton ?? (_recoverImageButton = new DelegateCommand((param) => {
                    if (param is ImageClientData)
                    {
                        ImageClientData data = param as ImageClientData;
                        data.IsDeleted = false;
                        CollectionViewSource.GetDefaultView(CurrentArticle.Images).Refresh();
                    }
                }));
            }
        }


        private DelegateCommand _saveArticleButton;
        public DelegateCommand SaveArticleButton
        {
            get
            {
                return _saveArticleButton ?? (_saveArticleButton = new DelegateCommand( (param) => { SaveArticle(); }));
            }
        }

        private async void SaveArticle()
        {
            //ArticleClientData article = new ArticleClientData(_currentArticle); //Create hard copy to preserve data
            try
            {
                await _service.SaveArticleAsync(_currentArticle);
                OnMessageApplication("Cikk mentése sikeres");
                ToList();
            }
            catch(Exception ex)
            {
                OnMessageApplication($"Hiba történt!\n{ex.Message}");
            }

        }

        private void OpenImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Images|*.jpg;*.jpeg;*.png";
            bool? result = dialog.ShowDialog();
            if(result.HasValue && result.Value)
            {
                string filepath = dialog.FileName;
                string name = dialog.SafeFileName;
                name = name.Substring(0, name.LastIndexOf('.')) ?? name;
                if(!IsImageFile(filepath))
                {
                    OnMessageApplication("Selected file is not a supported image file");
                    return;
                }              

                byte[] imageBuffer = File.ReadAllBytes(filepath);

                ImageClientData image = new ImageClientData();
                image.Base64 = Convert.ToBase64String(imageBuffer);
                image.Name = name;

                CurrentArticle.Images.Add(image);
            }

        }

        public EditViewModel(INewsService service)
        {
            _service = service;
        }

        private bool IsImageFile(string filePath)
        {
            Stream stream = File.OpenRead(filePath);
            stream.Seek(0, SeekOrigin.Begin);

            List<string> jpg = new List<string> { "FF", "D8" };
            //List<string> bmp = new List<string> { "42", "4D" };
            //List<string> gif = new List<string> { "47", "49", "46" };
            List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpg, png };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    return true;
                }
            }

            return false;
        }


        public async Task<bool> LoadAsync(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    ArticleClientData clientArticle = await _service.GetArticleAsync(id.Value);

                    CurrentArticle = clientArticle;

                }
                catch (Exception ex)
                {
                    OnMessageApplication($"Váratlan hiba történt! ({ex.Message})");
                    return false;
                }
            }

            else
            {
                CurrentArticle = new ArticleClientData();
                CurrentArticle.Images = new ObservableCollection<ImageClientData>();
            }

            return true;
        }

        private void ToList()
        {
            BackToList(this,null);
        }
    }
}
