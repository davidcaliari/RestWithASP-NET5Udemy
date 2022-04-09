namespace RestWithASPNETUdemy.Data.Contract
{
    public interface IParse<O, D>
    {
        D Parse(O origin);
        List<D> ParseList(List<O> origin);
    }
}
