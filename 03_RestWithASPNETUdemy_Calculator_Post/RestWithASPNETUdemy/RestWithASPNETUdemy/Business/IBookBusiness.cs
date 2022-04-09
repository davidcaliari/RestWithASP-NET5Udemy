using RestWithASPNETUdemy.Model;

namespace RestWithASPNETUdemy.Business
{
    public interface IBookBusiness
    {
        Book Create(Book person);
        Book FindByID(long id);
        List<Book> FindAll();
        Book Update(Book person);
        void Delete(long id);
    }
}
