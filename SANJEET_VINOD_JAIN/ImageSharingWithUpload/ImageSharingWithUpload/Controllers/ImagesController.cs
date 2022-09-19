using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using ImageSharingWithUpload.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

// TODO add annotations for HTTP actions -DONE

namespace ImageSharingWithUpload.Controllers
{
    using SysIOFile = System.IO.File;
    public class ImagesController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        private readonly ILogger logger;

        public ImagesController(IWebHostEnvironment environment, ILogger<ImagesController> logger)
        {
            hostingEnvironment = environment;
            this.logger = logger;
        }

        #region Internal Functions

        protected void mkDirectories()
        {
            var dataDir = Path.Combine(hostingEnvironment.WebRootPath,
               "data", "images");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            var infoDir = Path.Combine(hostingEnvironment.WebRootPath,
               "data", "info");
            if (!Directory.Exists(infoDir))
            {
                Directory.CreateDirectory(infoDir);
            }
        }

        protected string imageDataFile(string id)
        {
            return Path.Combine(
               hostingEnvironment.WebRootPath,
               "data", "images", id + ".jpg");
        }

        protected string imageInfoFile(string id)
        {
            return Path.Combine(
               hostingEnvironment.WebRootPath,
               "data", "info", id + ".json");
        }


        protected void CheckAda()
        {
            var cookie = Request.Cookies["ADA"];
            logger.LogInformation("ADA cookie value: " + cookie);
            if (cookie != null && "true".Equals(cookie))
            {
                ViewBag.isADA = true;
            }
            else
            {
                ViewBag.isADA = false;
            }
        }

        #endregion

        // TODO -DONE
        [HttpGet]
        public IActionResult Upload()
        {
            CheckAda();
            ViewBag.Message = "";
            return View();
        }

        // TODO -DONE
        [HttpPost]
        public async Task<IActionResult> Upload(Image image,
                                    IFormFile imageFile)
        {
            try
            {


                ViewBag.anyFileUpload = true;

                CheckAda();

                if (ModelState.IsValid)
                {
                    var UserId = Request.Cookies["UserId"];
                    if (UserId == null)
                    {
                        logger.LogInformation("Unregistered User");

                        return RedirectToAction("Register", "Account");
                    }

                    image.Userid = UserId;

                    /*
                     * Save image information on the server file system.-DONE
                     */

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        mkDirectories();

                        String fileName = image.Id;

                        // TODO save image and metadata-DONE
                        if (imageFile.ContentType != "image/jpeg")
                        {

                            // Error on content type; but can be faked! TODO - DONE
                            ViewBag.anyFileUpload = false;
                            ViewBag.errorFileUpload = "Please Upload a JPEG Image File";
                            logger.LogInformation("Incorrect Image File Type Upload : " + imageFile.ContentType);

                            return View(image);
                        }
                        else
                        {
                            using (FileStream DestinationStream = SysIOFile.Create(imageDataFile(image.Id)))
                            {
                                await imageFile.CopyToAsync(DestinationStream);
                            }
                            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                            String jsonData = JsonSerializer.Serialize(image, jsonOptions);
                            await SysIOFile.WriteAllTextAsync(imageInfoFile(fileName), jsonData);
                        }

                        return View("Details", image);
                    }
                    else
                    {
                        ViewBag.anyFileUpload = false;
                        ViewBag.errorFileUpload = "No image file specified!";
                        return View(image);
                    }

                }
                else
                {

                    ViewBag.anyFileUpload = false ;
                    ViewBag.errorFileUpload = "Upload an image";

                    if (ModelState["DateTaken"].Errors.Count > 0)
                    {

                        ModelState["DateTaken"].Errors.Clear();
                        ModelState.AddModelError("DateTaken", "Please Enter Valid Date");
                    }
                    ViewBag.Message = "Please correct the errors in the form!";

                    foreach (var item in ModelState)
                    {
                        if (item.Value.Errors.Count > 0)
                            logger.LogInformation("Error For : " + item.Key + ", Error Message : " + item.Value.Errors[0].ErrorMessage);
                    }
                    return View(image);
                }
            }
            catch (Exception e)
            {
                logger.LogError(1, e, "Error");
                return View(new ErrorViewModel { RequestId =null });
            }
        }

        // TODO-DONE
        [HttpGet]
        public IActionResult Query()
        {
            CheckAda();
            ViewBag.Message = "";
            return View();
        }

        // TODO-DONE
        [HttpGet]
        public async Task<IActionResult> Details(Image image)
        {
            CheckAda();

            var UserId = Request.Cookies["UserId"];
            if (UserId == null)
            {
                return RedirectToAction("Register", "Account");
            }

            String fileName = imageInfoFile(image.Id);
            if (System.IO.File.Exists(fileName))
            {
                String jsonData = await System.IO.File.ReadAllTextAsync(fileName);
                Image imageInfo = JsonSerializer.Deserialize<Image>(jsonData);

                return View(imageInfo);
            }
            else
            {
                ViewBag.Message = "Image with identifer " + image.Id + " not found";
                ViewBag.Id = image.Id;

                return View("Query");
            }

        }

    }
}
