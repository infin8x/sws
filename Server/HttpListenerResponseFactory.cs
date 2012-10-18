using System;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.Win32;

namespace Server
{
    static internal class HttpListenerResponseFactory
    {
        internal static void CreateOk(HttpListenerResponse response, FileInfo fileInfo, long length, String connection)
        {
            FillGeneralHeader(response, connection);
            response.ProtocolVersion = Protocol.Version;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.StatusDescription = Protocol.OkText;
            response.AddHeader(Protocol.LastModified, fileInfo.LastWriteTimeUtc.ToString(Protocol.UtcTimeFormatString));
            response.AddHeader(Protocol.ContentLength, length.ToString(CultureInfo.InvariantCulture));

            var mimeType = GetMimeType(fileInfo);
            if (mimeType != "") response.AddHeader(Protocol.ContentType, mimeType);
        }

        internal static void CreateNotFound(HttpListenerResponse response, String connection)
        {
            FillGeneralHeader(response, connection);
            response.StatusCode = (int)HttpStatusCode.NotFound;
            response.StatusDescription = Protocol.NotFoundText;
            
        }

        internal static void FillGeneralHeader(HttpListenerResponse response, String connection)
        {
            response.AddHeader(Protocol.Connection, connection);
            response.AddHeader(Protocol.Date, DateTime.UtcNow.ToString(Protocol.UtcTimeFormatString));
            response.AddHeader(Protocol.Server, Protocol.GetServerInfo());
            response.AddHeader(Protocol.Provider, Protocol.Author);
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