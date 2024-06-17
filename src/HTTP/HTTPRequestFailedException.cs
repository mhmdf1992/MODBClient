using System;

namespace MO.MODBClient.HTTP{
    public class HTTPRequestFailedException : Exception{
        private string _response;
        private int _statusCode;
        public string Response => _response;
        public int StatusCode => _statusCode;
        private string _statusMessage;
        public string StatusMessage => _statusMessage;
        public HTTPRequestFailedException(Exception inner, string response, int statusCode, string statusMessage) : base("Status Code does not indicate success", inner){
            _response = response;
            _statusCode = statusCode;
            _statusMessage = statusMessage;
        }
    }
}