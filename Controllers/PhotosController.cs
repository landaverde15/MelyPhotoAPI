using Microsoft.AspNetCore.Mvc;
using MelyPhotography.Models;
using System.Diagnostics;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System;
using System.IO;

namespace MelyPhotography.Controllers
{
    [Route("api/MelyPhotos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public PhotosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Public API Methods
        /// <summary>
        /// Uploads image to DB
        /// </summary>
        /// <param name="file">IFormFile file</param>
        /// <returns>instance of UploadDTO object</returns>
        [HttpPost]
        [Route("UploadImage")]
        [ActionName("UploadImage")]
        public async Task<UploadResultDTO> UploadFile(IFormFile file, string passcodeID)
        {
            UploadResultDTO result = new UploadResultDTO();
            byte[] byteImage = new byte[] { };
            
            try
            {
                if (passcodeID == Environment.GetEnvironmentVariable("APPSETTING_PasscodeID").ToString())
                {
                    if (file == null || file.Length == 0)
                    {
                        result.Message = "Image not submitted";
                        result.Success = false;
                    }
                    else
                    {

                        using (var stream = new MemoryStream())
                        {
                            file.CopyTo(stream);
                            byteImage = stream.ToArray();
                        }
                        BsonDocument doc = new BsonDocument
                        {
                            { "photo", Convert.ToBase64String(byteImage) }
                        };
                        MongoClient client = GetMongoDBConnection();
                        var collection = client.GetDatabase(Environment.GetEnvironmentVariable("APPSETTING_MongoDB")).GetCollection<BsonDocument>(this._configuration.GetValue<string>("MongoCollection"));
                        InsertOneOptions options = new InsertOneOptions() { BypassDocumentValidation = true };
                        await collection.InsertOneAsync(doc, options);

                        result.Message = "Image saved";
                        result.Success = true;
                    }


                } else
                {
                    result.Success = false;
                    result.Message = "Access Denied";
                }


            } catch(Exception ex) 
            {
                result.Success = false;
                result.Message = "Something went wrong";
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
        public GetAllPhotosDTO GetImages()
        {
            GetAllPhotosDTO result = new GetAllPhotosDTO();
            try
            {
                MongoClient client = GetMongoDBConnection();
                IMongoCollection<PhotoDTO> collection = client.GetDatabase(Environment.GetEnvironmentVariable("APPSETTING_MongoDB")).GetCollection<PhotoDTO>(this._configuration.GetValue<string>("MongoCollection"));
                IMongoQueryable<PhotoDTO> query = collection.AsQueryable()
                    .Select(image => image);

                result.Photos = query;
                result.Success = true;
            } catch(Exception ex)
            {
                result.Success = false;
                Trace.WriteLine(ex.Message);
            }

            return result;
        }
        #endregion

        #region MongoDB Methods
        private MongoClient GetMongoDBConnection()
        {
            MongoClient client = new MongoClient();

            try
            {
                string connectionString = Environment.GetEnvironmentVariable("APPSETTING_MongoDBConnectionString");
                client = new MongoClient(connectionString);

            } catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            return client;
        }
        #endregion
    }
}
