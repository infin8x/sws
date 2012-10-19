using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Server.Protocol
{
    class HttpRequest
    {
        public String Method { get; private set; }
        public String Uri { get; private set; }
        public Version ProtocolVersion { get; private set; }
        public Dictionary<String, String> Headers { get; private set; }

        public HttpRequest(StreamReader input)
        {
            Headers = new Dictionary<String, String>();

            try
            {
                ParseRequest(input.ReadLine());
                while (input.Peek() > 0)
                    ParseHeader(input.ReadLine().Trim());
            }
            catch (Exception e)
            {
                throw new ProtocolException(Constants.BadRequestCode, Constants.BadRequestText, e);
            }
        }

        private void ParseHeader(string readLine)
        {
            if (readLine.Length == 0) return;
            var tokens = readLine.Split(' ');
            Headers.Add(tokens[0].Substring(0, tokens[0].Length - 1),
                readLine.Substring(tokens[0].Length));
        }

        private void ParseRequest(string readLine)
        {
            var tokens = readLine.Split(' ');
            if (tokens.GetLength(0) != 3)
            {
                throw new ProtocolException(Constants.BadRequestCode, Constants.BadRequestText);
            }
            Method = tokens[0];
            Uri = tokens[1];
            ProtocolVersion = Constants.GetHttpVersion(tokens[2]);
        }
    }
}
