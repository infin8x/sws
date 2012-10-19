using System;
using System.Net;

namespace Server.Protocol
{
    internal class Constants
    {
        // Escape characters
        public const char Space = ' ';
        public const char Seperator = ':';
        public const char Slash = '/';
        public const char CR = '\r';
        public const char LF = '\n';
        public const String CRLF = "\r\n";

        // Some useful protocol elements
        public static Version Version = HttpVersion.Version11;
        public const String Get = "GET";
        public const String UtcTimeFormatString = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";

        // Some useful http code text
        public const int OkCode = 200;
        public const String OkText = "OK";

        public const int MovedPermanentlyCode = 301;
        public const String MovedPermanentlyText = "Moved Permanently";

        public const int BadRequestCode = 400;
        public const String BadRequestText = "Bad Request";

        public const int NotFoundCode = 404;
        public const String NotFoundText = "Not Found";

        public const int NotSupportedCode = 505;
        public const String NotSupportedText = "HTTP Version Not Supported";

        // Some useful header elements in request
        public const String Host = "Host";
        public const String Connection = "Connection";
        public const String UserAgent = "User-Agent";

        // Some useful header elements in response
        public const String Date = "Date";
        public const String Server = "Server";
        public const String LastModified = "Last-Modified";
        public const String ContentLength = "Content-Length";
        public const String ContentType = "Content-Type";

        /**
         * A chunk size to be used when reading a file and sending it to a socket. 
         * Rather than reading the whole file at once, we divide the reading of the file
         * into number of small chunk of bytes and send each chunk to the socket to 
         * utilize the memory better.
         */
        public const int ChunkLength = 4096; // 4KB

        // Server information that we want to send in "Server:" header field
        public const String ServerInfo = "Server/1.0.0";
        public const String Provider = "Provider";
        public const String Author = "Alexander J Mullans & Kurtis A Zimmerman";
        public const String Close = "Close";
        public const String Open = "Keep-Alive";
        public const String DefaultFile = "index.html";
        public const String MimeText = "text";


        public static String GetServerInfo()
        {
            var os = Environment.OSVersion.VersionString;
            var architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            return ServerInfo + Space + "(" + os + Slash + architecture + ")";
        }

        public static Version GetHttpVersion(string rawVersion)
        {
            if (rawVersion == "HTTP/1.0")
                return HttpVersion.Version10;
            if (rawVersion == "HTTP/1.1")
                return HttpVersion.Version11;
            // TODO: protocol exception
            return null;
        }
    }
}