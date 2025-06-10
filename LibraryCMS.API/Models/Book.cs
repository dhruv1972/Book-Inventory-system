using System.Collections.Generic;

namespace LibraryCMS.API.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; }
        public ICollection<BookCategory> BookCategories { get; set; }
    }
}
