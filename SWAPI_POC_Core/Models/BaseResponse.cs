using SWAPI_POC_Core.Interfaces.Base;

namespace SWAPI_POC_Core.Models
{
    public class BaseResponse : IBaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
