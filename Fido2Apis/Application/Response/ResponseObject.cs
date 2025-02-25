namespace Fido2Apis.Application.Response
{
    public class ResponseObject
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public int code { get; set; }
    }
}
