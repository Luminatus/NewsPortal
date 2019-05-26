using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.Persistence.DTO;
using System.IO;
using NewsPortal.DesktopApplication.ViewModel;
using System.ComponentModel;

namespace NewsPortal.DesktopApplication.Model
{
    public class ImageClientData : ImageDTO
    {
        public Boolean IsDeleted { get; set; } = false;
        public Boolean IsUploaded { get; set; } = false;

        public ImageClientData() : base() { }
        public ImageClientData(ImageDTO data):base()
        {
            Id = data.Id;
            Base64 = data.Base64;
            Name = data.Name;
        }

    }
}
