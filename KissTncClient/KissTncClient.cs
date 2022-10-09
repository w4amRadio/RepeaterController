using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace KissTncClient
{
    public class KissTncClient : IDisposable
    {
        private TcpClient _client;
        private NetworkStream stream = null;
        private bool _isTransmissing = false;
        private ILogger _logger;

        private byte[] _messageBuffer = new byte[2048];
        private List<byte> message = new List<byte>();

        //public event DataReceivedEvent DataReceived;
        public event EventHandler<EventArgs> DataReceived;


        public KissTncClient(
            ILogger logger,
            string address,
            int port)
        {
            if(_logger == null)
            {
                throw new ArgumentNullException($"Logger passed in to KissTncClient must not be null!");
            }

             _client = new TcpClient(address, port);

            if (_client.Connected)
            {
                stream = _client.GetStream();
                stream.BeginRead(_messageBuffer, 0, _messageBuffer.Length, new AsyncCallback(ReadCallback), stream);
            }
            

        }

        public KissTncClient(string serialPort, int baudRate)
        {
            throw new NotImplementedException("Serial port interface not yet implemented for KissTncClient!");
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            int bytesRead = 0;
            try
            {
                bytesRead = stream.EndRead(asyncResult);
            }
            catch(IOException ioex)
            {
                //our connection to our software tnc (e.g., soundmodem or direwolf) was lost

            }

            if (bytesRead == 0)
                return;

            Span<byte> span = new Span<byte>(_messageBuffer, 0, bytesRead);
            message.AddRange(span.ToArray());

            if(!stream.DataAvailable)
            {
                //DataReceived(message);
            }

            //make recursive call to ReadCallback until the message is over
        }

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }

            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }
    } 
}
