using System;
using System.IO;
using System.Net;
using Microsoft.Win32;

namespace Server.Protocol
{
    static internal class HttpResponseFactory
    {
        internal static HttpResponse CreateOk(String file, String connection)
        {
            var response = new HttpResponse(Constants.Version, (int)HttpStatusCode.OK, Constants.OkText);
            FillGeneralHeader(response, connection);

            response.RequestedFilePath = file;
            var fileInfo = new FileInfo(file);
            response.AddHeader(Constants.ContentLength, fileInfo.Length.ToString());
            response.AddHeader(Constants.LastModified, fileInfo.LastWriteTimeUtc.ToString(Constants.UtcTimeFormatString));


            var mimeType = GetMimeType(fileInfo);
            if (mimeType != "") response.AddHeader(Constants.ContentType, mimeType);

            return response;
        }

        internal static HttpResponse CreateNotFound(String connection)
        {
            var response = new HttpResponse(Constants.Version, Constants.NotFoundCode, Constants.NotFoundText);
            FillGeneralHeader(response, connection);
            return response;
        }

        internal static HttpResponse CreateBadRequest(String connection)
        {
            var response = new HttpResponse(Constants.Version, Constants.BadRequestCode, Constants.BadRequestText);
            FillGeneralHeader(response, connection);
            return response;
        }

        internal static HttpResponse CreateNotSupported(String connection)
        {
            var response = new HttpResponse(Constants.Version, Constants.NotSupportedCode, Constants.NotSupportedText);
            FillGeneralHeader(response, connection);
            return response;
        }

        internal static HttpResponse CreateNotModified(String connection)
        {
            var response = new HttpResponse(Constants.Version, Constants.NotModifiedCode, Constants.NotModifiedText);
            FillGeneralHeader(response, connection);
            return response;
        }

        internal static void FillGeneralHeader(HttpResponse response, String connection)
        {
            response.AddHeader(Constants.Connection, connection);
            response.AddHeader(Constants.Date, DateTime.UtcNow.ToString(Constants.UtcTimeFormatString));
            response.AddHeader(Constants.Server, Constants.GetServerInfo());
            response.AddHeader(Constants.Provider, Constants.Author);
        }

        // GetMimeType method from: http://stackoverflow.com/a/1685614/297050
        private static string GetMimeType(FileInfo fileInfo)
        {
            var mimeType = "";

            var regKey = Registry.ClassesRoot.OpenSubKey(
                fileInfo.Extension.ToLower()
                );

            if (regKey != null)
            {
                var contentType = regKey.GetValue("Content Type");

                if (contentType != null)
                    mimeType = contentType.ToString();
            }
            return mimeType;
        }
    }
}