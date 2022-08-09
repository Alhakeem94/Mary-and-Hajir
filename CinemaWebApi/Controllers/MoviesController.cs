using DataAccessLayer.data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Models;
using ModelsLibrary.ViewModels;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        private ApplicationDbContext _db;
        private IWebHostEnvironment _env;
        public MoviesController(ApplicationDbContext db , IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        


        // Recieve Movie => MovieName =>MovieImage
        [HttpPost]
        [Route("AddMovie")]
        public async Task<IActionResult> AddMovie([FromForm]MoviesViewModel newMovie)
        {
            var MovieInDataBase = await _db.MoviesTable.SingleOrDefaultAsync(a => a.MovieName == newMovie.MovieName);

            if (MovieInDataBase is null)
            {

                var Model = new MoviesModel();

                Model.MovieName      = newMovie.MovieName;
                Model.MovieCatagory  = newMovie.MovieCatagory;
                Model.MovieRating    = newMovie.MovieRating;
                Model.MovieDuration  = newMovie.MovieDuration;
                Model.DirectorName   = newMovie.DirectorName;
                Model.YearOfRelease  = newMovie.YearOfRelease;
                Model.MainStarName   = newMovie.MainStarName;
                Model.MoviePhotoPath = await InputImage(newMovie.PhotoPath);
                Model.MovieVideoPath = newMovie.MovieVideoPath;

                await _db.MoviesTable.AddAsync(Model); 
                await _db.SaveChangesAsync();

                return Ok("The Movie Has been Added Successfuly to the database");
            }
            else
            {
                return BadRequest("the Movie Already exists in the DataBase");
            }

        }



        [NonAction]
        public async Task<string> InputImage(IFormFile e)
        {

            var fileInfo = new FileInfo(e.Name); 
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(e.FileName);
                           
            var folderdirectory = $"{_env.WebRootPath}\\Images";
            var path = Path.Combine(folderdirectory, filename);



            var memorystream = new MemoryStream();
            await e.OpenReadStream().CopyToAsync(memorystream);

            await using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                memorystream.WriteTo(fs);
            }
            var fullpath = $"https://localhost:7209/Images/{filename}";
            return fullpath;

        }













        [HttpGet]
        [Route("GetAllMovies")]
        public async Task<IActionResult> GetAllMovies()
        {
            var ListOfAllMovies = await _db.MoviesTable.ToListAsync();

            return Ok(ListOfAllMovies);
        }
      



        [HttpGet]
        [Route("GetAMovie")]
        public async Task<IActionResult> SearchAMovie(string MovieName)
        {
            var MovieInDataBase = await _db.MoviesTable.SingleOrDefaultAsync(a => a.MovieName == MovieName);
            if (MovieInDataBase is null)
            {
                return Ok("The Movie in not Available in the database");
            }
            else
            {
                return Ok(MovieInDataBase);
            }
        }

       
        [HttpGet]
        [Route("GetMoviesInACatagory")]
        public async Task<IActionResult> GetAllMoviesInACatagory(string catagory)
        {
            var ListOfMovies = await _db.MoviesTable.Where(a => a.MovieCatagory == catagory).ToListAsync();
            if (ListOfMovies.Count == 0)
            {
                return Ok("There are no movies for this Catagory");
            }
            else
            {
                return Ok(ListOfMovies);
            }
        }


        [HttpGet]
        [Route("GetMoviesByMainActor")]
        public async Task<IActionResult> GetAllMoviesByMainActor(string MainActor)
        {
            var ListOfMovies = await _db.MoviesTable.Where(a => a.MainStarName == MainActor).ToListAsync();

            var ListOfMoviesViewModel = new List<MoviesViewModel>();

            if (ListOfMovies.Count == 0)
            {
                return Ok("There are no movies for this actor");
            }
            else
            {
                foreach (var item in ListOfMovies)
                {
                    var viewModel = new MoviesViewModel();

                    viewModel.MovieName = item.MovieName;
                    viewModel.DirectorName = item.DirectorName;
                    viewModel.MovieDuration = item.MovieDuration;
                    viewModel.MovieCatagory = item.MovieCatagory;
                    viewModel.MovieRating = item.MovieRating;
                    viewModel.MainStarName = item.MainStarName;
                    viewModel.YearOfRelease = item.YearOfRelease;

                    ListOfMoviesViewModel.Add(viewModel);
                }

                return Ok(ListOfMoviesViewModel);
            }
        }



        [HttpGet]
        [Route("GetMovieById")]
        public async Task<IActionResult> GetMovieById(int MovieId)
        {
            var Movie = await _db.MoviesTable.SingleOrDefaultAsync(a => a.Id == MovieId);
            if (Movie is null)
            {
                return Ok("The Movie is Not Found in the database");
            }
            else
            { 
                return Ok(Movie);
            }
        }


        [HttpDelete]
        [Route("DeleteAMovie")]
        public async Task<IActionResult> DeleteAMovie(int MovieId)
        {
            try
            {
                var Movie = await _db.MoviesTable.SingleOrDefaultAsync(a => a.Id == MovieId);
                if (Movie is not null)
                {
                    _db.MoviesTable.Remove(Movie);
                    await _db.SaveChangesAsync();
                    return Ok("The Movie has been Deleted Successfuly");
                }
                else
                {
                    return Ok($"There is no movie with the Id : {MovieId} in the DataBase");
                }

            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

        [HttpPut]
        [Route("EditAMovie")]
        public async Task<IActionResult> EditAMovie(MoviesModel EditedMovie)
        {
            try
            {
                var MovieInDataBase = await _db.MoviesTable.SingleOrDefaultAsync(a => a.Id == EditedMovie.Id);


                MovieInDataBase.MovieName = EditedMovie.MovieName;
                MovieInDataBase.DirectorName = EditedMovie.DirectorName;
                MovieInDataBase.MovieCatagory = EditedMovie.MovieCatagory;
                MovieInDataBase.MovieRating = EditedMovie.MovieRating;
                MovieInDataBase.MovieDuration = EditedMovie.MovieDuration;
                MovieInDataBase.YearOfRelease = EditedMovie.YearOfRelease;

                _db.MoviesTable.Update(MovieInDataBase);
                 await _db.SaveChangesAsync();
                return Ok("the Movie has been updated Successfuly");

            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }  
        

    }
















}
