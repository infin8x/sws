using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Protocol
{
    class HttpResponse
    {
        public String Method { get; private set; }
        public String Uri { get; private set; }
        public Version ProtocolVersion { get; private set; }
        public int Status { get; private set; }
        public string Message { get; private set; }
        public string RequestedFilePath { get; set; }
        public Dictionary<String, String> Headers { get; private set; }

        public HttpResponse(Version version, int status, String message)
        {
            ProtocolVersion = version;
            Status = status;
            Message = message;
            Headers = new Dictionary<string, string>();
        }

        public void AddHeader(String key, String value)
        {
            Headers.Add(key, value);
        }

        public void Write(Stream output)
        {
            var line = "HTTP/" + ProtocolVersion + Constants.Space +
                       Status + Constants.Space + Message + Constants.CRLF;
            output.Write(Encoding.UTF8.GetBytes(line), 0, Encoding.UTF8.GetByteCount(line));
            foreach (var header in Headers)
            {
                line = header.Key + Constants.Seperator + Constants.Space + header.Value + Constants.CRLF;
                output.Write(Encoding.UTF8.GetBytes(line), 0, Encoding.UTF8.GetByteCount(line));
            }
            line = Constants.CRLF;
            output.Write(Encoding.UTF8.GetBytes(line), 0, Encoding.UTF8.GetByteCount(line));

            if (Status == Constants.OkCode)
            {
                WriteFileToOutputStream(output);
            }
            output.Close();
        }

        private void WriteFileToOutputStream(Stream output)
        {
            using (var requestedFile = File.Open(RequestedFilePath, FileMode.Open))
            {
                var buffer = new byte[Constants.ChunkLength];
                var bytesToRead = requestedFile.Length;
                var bytesRead = 0;
                while (bytesToRead > 0)
                {
                    var n = requestedFile.Read(buffer, bytesRead, Constants.ChunkLength);
                    if (n == 0) break; // EOF
                    bytesRead += n;
                    bytesToRead -= n;
                    output.Write(buffer, 0, bytesRead);
                }
                output.Flush();
            }
        }
    }
}
