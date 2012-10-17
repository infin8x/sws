using System.Net;

namespace Server
{
    static internal class HttpListenerResponseFactory
    {
        public static void CreateOk(HttpListenerResponse response)
        {
            response.StatusCode = (int) HttpStatusCode.OK;
            response.ContentType = "text/html";
            response.ContentLength64 = 0;
        }

        internal static void CreateNotFound(HttpListenerResponse response)
        {
            response.StatusCode = (int) HttpStatusCode.NotFound;
            response.ContentType = "text/html";
            response.ContentLength64 = 0;
        }
    }
}