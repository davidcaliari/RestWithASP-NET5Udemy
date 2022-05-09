using RestWithASPNETUdemy.Data.Converter.VO;

namespace RestWithASPNETUdemy.Business
{
    public interface ILoginBusiness
    {
        TokenVO ValidateCredentials(UserVO user);
    }
}
