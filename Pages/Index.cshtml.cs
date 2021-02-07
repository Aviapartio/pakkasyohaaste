using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Pakkasyöhaaste.CloudStorage;

namespace Pakkasyöhaaste.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IHostingEnvironment _environment;
        private IConfiguration _configuration;
        private ICloudStorage _cloudStorage;

        public IndexModel(ILogger<IndexModel> logger,IHostingEnvironment environment, IConfiguration configuration,
                        ICloudStorage cloudStorage)
        {
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
            _cloudStorage = cloudStorage;
        }
        public void OnGet()
        {
            Pvm = DateTime.Today;
        }
        [BindProperty]
        [Required]
        public IFormFile Upload { get; set; }

        [BindProperty]
        public String Password { get; set; }

        [BindProperty]
        [Required]
        public String Nimi { get; set; }
        [BindProperty]
        public String Kuvaus { get; set; }
        [BindProperty]
        public String Pakkasta { get; set; }

        [BindProperty]
        [Required]
        [DisplayFormat(DataFormatString = "{0:d.M.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]

        public DateTime Pvm { get; set; }

        [DisableRequestSizeLimit]
        public async Task OnPostAsync()
        {
            ViewData["ErrorMessage"] = "";
            ViewData["Success"] = "0";
            if ((Password != null) && (Password.ToLower() == _configuration["Salasana"].ToLower())) {
                if (Nimi != null &&  Upload !=null && ((Upload.FileName!=null) && (Nimi.Length > 1)))
                {
                    var thisdate = DateTime.Now.ToString("yyyyMMdd_HHmm_");
                    await _cloudStorage.UploadFileAsync(Upload, thisdate + Upload.FileName);
                    var contentfilename = Path.ChangeExtension(Upload.FileName, ".txt");
                    var kuvaus = $"Nimi: {Nimi}\nPakkasta: {Pakkasta}\nPvm: {Pvm:d.M.yyyy}\nKuvaus: {Kuvaus}";
                    await _cloudStorage.UploadStringAsync(kuvaus, thisdate+contentfilename);

                    ViewData["ErrorMessage"] = "Kiitos osallistumisesta pakkasyöhaasteeseen";
                    ViewData["Success"] = "1";

                }
                else
                {
                    ViewData["ErrorMessage"] = "Täytä nimesi ja kuva vähintään";
                }
            }
            else
            {
                ViewData["ErrorMessage"] = " Salasana oli väärin. Saat sen ryhmänjohtajaltasi";

            }
        }
    }
}

