using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Display(Name="Наименование")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Год выпуска")]
        [DataType(DataType.Date)]
        public DateTime Year_of_release { get; set; }
        [Display(Name = "Режиссер")]
        public string Producer { get; set; }
        public User User { get; set; }
        [Display(Name = "Постер")]
        public string Poster { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
