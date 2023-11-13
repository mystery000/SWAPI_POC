using SWAPI_POC_Core.Interfaces.Base;

namespace SWAPI_POC_Core.Models
{
    public class DataResponse<T> : BaseResponse, IDataResponse<T>
    {
        public T Data { get; set; }
    }
}
