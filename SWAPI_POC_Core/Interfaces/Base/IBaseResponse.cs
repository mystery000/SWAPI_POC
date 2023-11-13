namespace SWAPI_POC_Core.Interfaces.Base
{
    public interface IBaseResponse
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}
