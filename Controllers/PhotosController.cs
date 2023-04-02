using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MelyPhotography.Models;
using System.Diagnostics;

namespace MelyPhotography.Controllers
{
    [Route("api/MelyPhotos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {

        private Microsoft.AspNetCore.Hosting.IHostingEnvironment Environment;
        public PhotosController(Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment)
        {
            Environment = _environment;
        }

        /// <summary>
        /// Uploads image(s) to server
        /// </summary>
        /// <param name="file">IFormFile file(s)</param>
        /// <returns>instance of UploadDTO object</returns>
        [HttpPost]
        [Route("UploadImage")]
        [ActionName("UploadImage")]
        public async Task<UploadDTO> UploadFiles(List<IFormFile> files)
        {
            UploadDTO result = new UploadDTO();
            List<string> uploadedFiles = new List<string>();
            string wwwPath = string.Empty;
            string contentPath = string.Empty;
            string path = string.Empty;

            try
            {
                wwwPath = this.Environment.WebRootPath;
                contentPath = this.Environment.ContentRootPath;
                path = Path.Combine(this.Environment.WebRootPath, "Uploads");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                foreach(IFormFile file in files)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        file.CopyTo(stream);
                        uploadedFiles.Add(fileName);
                    }
                }

                result.files = uploadedFiles;
                result.success = true;


            } catch(Exception ex) 
            {
                result.success = false; 
                Trace.WriteLine(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Get all images from server
        /// </summary>
        /// <returns>instance of GetPhotosDTO</returns>
        [HttpGet]
        [Route("GetImages")]
        [ActionName("GetImages")]
        public async Task<GetPhotosDTO> GetImages()
        {
            GetPhotosDTO result = new GetPhotosDTO();
            List<string> images = new List<string>();

            try
            {
                string[] files = Directory.GetFiles(Path.Combine(this.Environment.WebRootPath, "Uploads/"));
                foreach (string file in files)
                {
                    images.Add(file);
                }
                result.photos = images;
                result.success = true;
            } catch(Exception ex)
            {
                result.success = false;
                Trace.WriteLine(ex.Message);
            }

            return result;
        }
    }
}
