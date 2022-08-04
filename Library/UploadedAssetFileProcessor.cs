using Library.Models.Catalog;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Library
{
    public abstract class UploadedAssetFileProcessor
    {
        public string ProcessUploadedAssetFile(AssetCreateViewModel model, IWebHostEnvironment webHostEnvironment)
        {
            string uniqueFileName = null;

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
