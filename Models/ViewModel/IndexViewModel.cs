using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Models.ViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<Movie> Movies { get; set; }
        public PageViewModel PageViewModel { get; set; }

        public bool IsEmail { get; set; }
    }
}
