namespace SWAPI_POC_Core.Interfaces.Base
{
    public interface IDataResponse<T> : IBaseResponse
    {
        T Data { get; set; }
    }
}
