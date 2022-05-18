using RestWithASPNETUdemy.Data.Converter.Contract.Implementations;
using RestWithASPNETUdemy.Data.VO;
using RestWithASPNETUdemy.Hypermedia.Utils;
using RestWithASPNETUdemy.Model;
using RestWithASPNETUdemy.Repository;

namespace RestWithASPNETUdemy.Business.Implementations
{
    public class BookBusinessImplementation : IBookBusiness
    {

        private readonly IRepository<Book> _repository;
        private readonly BookConverter _converter;

        public BookBusinessImplementation(IRepository<Book> repository/*, BookConverter converter*/)
        {
            _repository = repository;
            _converter = new BookConverter();
        }

        public List<BookVO> FindAll()
        {

            return _converter.ParseList(_repository.FindAll());
        }

        public PagedSearchVO<BookVO> FindWithPagedSearch(
            string title, string sortDirection, int pageSize, int page)
        {
            var sort = (!string.IsNullOrWhiteSpace(sortDirection)) && !sortDirection.Equals("desc") ? "asc" : "desc";
            var size = (pageSize < 1) ? 10 : pageSize;
            var offset = page > 0 ? (page - 1) * size : 0;

            string query = @"SELECT * FROM books p WHERE 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(title)) query += $" AND p.title like '%{title}%' ";
            query += $"ORDER BY p.title {sort} OFFSET {offset} ROWS FETCH NEXT {size} ROWS ONLY";

            string countQuery = @"SELECT COUNT(*) FROM books p WHERE 1 = 1 ";
            if (!string.IsNullOrWhiteSpace(title)) countQuery += $" AND p.title like '%{title}%' ";

            var books = _repository.FindWithPagedSearch(query);
            int totalResults = _repository.GetCount(countQuery);

            return new PagedSearchVO<BookVO>
            {
                CurrentPage = page,
                List = _converter.ParseList(books),
                PageSize = size,
                SortDirections = sort,
                TotalResults = totalResults
            };
        }

        public BookVO FindByID(long id)
        {
            return _converter.Parse(_repository.FindByID(id));
        }

        public BookVO Create(BookVO book)
        {
            var bookEntity = _converter.Parse(book);
            return _converter.Parse(_repository.Create(bookEntity));
        }

        public BookVO Update(BookVO book)
        {
            var bookEntity = _converter.Parse(book);
            return _converter.Parse(_repository.Update(bookEntity));
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        
    }
}
