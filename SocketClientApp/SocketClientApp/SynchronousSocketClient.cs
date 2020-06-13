namespace SocketClientApp
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Windows.Forms;

    class SynchronousSocketClient
    {
        private static readonly int HEADER_SIZE = 10;

        public static void StartClient(RichTextBox logTextBox)
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                IPAddress ipAddress = IPAddress.Parse("192.168.1.110");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5555);

                // Create a TCP/IP  socket.  
                Socket socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    socket.Connect(remoteEP);

                    logTextBox.AppendText(string.Format("Socket connected to {0}",
                        socket.RemoteEndPoint.ToString()));

                    for (int i = 0; i < 5; i++)
                    {
                        ReadMessage(socket, logTextBox);
                    }

                    // Release the socket.  
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                }
                catch (ArgumentNullException ane)
                {
                    logTextBox.AppendText(string.Format("\nArgumentNullException : {0}", 
                        ane.ToString()));
                }
                catch (SocketException se)
                {
                    logTextBox.AppendText(string.Format("\nSocketException : {0}", 
                        se.ToString()));
                }
                catch (Exception e)
                {
                    logTextBox.AppendText(string.Format("\nUnexpected exception : {0}", 
                        e.ToString()));
                }

            }
            catch (Exception e)
            {
                logTextBox.AppendText("\n" + e.ToString());
            }
        }

        private static void ReadMessage(Socket socket, RichTextBox logTextBox)
        {
            // Encode the data string into a byte array.  
            byte[] buffer = new byte[16];
            int msgLenght = -1;
            StringBuilder sb = new StringBuilder();
            while (msgLenght < 0 || sb.Length < (msgLenght + HEADER_SIZE))
            {
                // Receive the response from the remote device.  
                int bytesRec = socket.Receive(buffer);
                string package = Encoding.ASCII.GetString(buffer, 0, bytesRec);
                logTextBox.AppendText(string.Format("\nReceived = {0}", package));
                sb.Append(package);
                if (msgLenght < 0)
                {
                    msgLenght = int.Parse(package.Substring(0, HEADER_SIZE));
                }
            }

            logTextBox.AppendText(string.Format("\nMessage is: {0}", sb.ToString()));
        }
    }
}
