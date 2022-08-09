using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.ViewModels
{
    public class MoviesViewModel
    {
        public string MovieName { get; set; }
        public string MovieCatagory { get; set; }
        public decimal MovieRating { get; set; }
        public int YearOfRelease { get; set; }
        public string MainStarName { get; set; }
        public string DirectorName { get; set; }
        public string MovieDuration { get; set; }
        public IFormFile PhotoPath { get; set; }
        public string MovieVideoPath { get; set; }


    }
}
