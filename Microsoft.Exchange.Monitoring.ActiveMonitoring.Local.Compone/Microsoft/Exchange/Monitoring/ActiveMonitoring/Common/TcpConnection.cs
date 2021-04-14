using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public abstract class TcpConnection : IDisposable
	{
		public TcpConnection(EndPoint targetEndpoint)
		{
			bool flag = false;
			Socket socket = null;
			try
			{
				socket = new Socket(targetEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(targetEndpoint);
				this.connection = new NetworkConnection(socket, 4096);
				this.connection.SendTimeout = 120;
				this.connection.ReceiveTimeout = 120;
				this.data = new byte[4096];
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					if (this.connection != null)
					{
						this.connection.Dispose();
					}
					else if (socket != null)
					{
						try
						{
							if (socket.Connected)
							{
								socket.Shutdown(SocketShutdown.Both);
							}
						}
						finally
						{
							socket.Close();
						}
					}
				}
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this.connection.LocalEndPoint;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.connection.RemoteEndPoint;
			}
		}

		internal ChannelBindingToken ChannelBindingToken
		{
			get
			{
				return this.connection.ChannelBindingToken;
			}
		}

		protected byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		public void SendRawData(byte[] data, int offset, int length)
		{
			IAsyncResult asyncResult = this.connection.BeginWrite(data, offset, length, null, null);
			if (!asyncResult.AsyncWaitHandle.WaitOne(120000, false))
			{
				throw new TimeoutException(string.Format("No data written in {0} seconds.", 120));
			}
			object obj;
			this.connection.EndWrite(asyncResult, out obj);
		}

		public void SendData(byte[] data)
		{
			this.SendData(data, 0, data.Length);
		}

		public void SendData(byte[] data, int offset, int length)
		{
			if (data[length - 2] != TcpConnection.byteCrLf[0] && data[length - 1] != TcpConnection.byteCrLf[1])
			{
				throw new ArgumentException("SendData must end with CRLF.");
			}
			this.SendRawData(data, offset, length);
		}

		public void SendData(string request)
		{
			string text = request;
			if (!text.EndsWith("\r\n"))
			{
				text = string.Format("{0}{1}", text, "\r\n");
			}
			byte[] array = new byte[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				array[i] = (byte)(text[i] & 'ÿ');
			}
			this.SendData(array);
		}

		public TcpResponse SendDataWithResponse(string data)
		{
			this.SendData(data);
			return this.GetResponse(120, null, false);
		}

		public TcpResponse GetResponse()
		{
			return this.GetResponse(120, null, false);
		}

		public TcpResponse GetResponse(int timeout, string expectedTag, bool multiLine)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string responseString = string.Empty;
			do
			{
				byte[] rawBytes = this.GetRawBytes(timeout);
				if (rawBytes.Length == 0)
				{
					break;
				}
				stringBuilder.Append(Encoding.ASCII.GetString(rawBytes, 0, rawBytes.Length));
				responseString = stringBuilder.ToString();
			}
			while (!this.LastLineReceived(responseString, expectedTag, multiLine));
			return this.CreateResponse(responseString);
		}

		public string GetRawString()
		{
			byte[] rawBytes = this.GetRawBytes(120);
			string result;
			if (rawBytes.Length == 0)
			{
				result = string.Empty;
			}
			else
			{
				result = Encoding.ASCII.GetString(rawBytes, 0, rawBytes.Length);
			}
			return result;
		}

		public IAsyncResult BeginRead()
		{
			return this.connection.BeginRead(null, null);
		}

		public string EndRead(IAsyncResult asyncResult)
		{
			byte[] array;
			int srcOffset;
			int num;
			object obj;
			this.connection.EndRead(asyncResult, out array, out srcOffset, out num, out obj);
			if (obj != null)
			{
				throw new ApplicationException("EndRead() resulted in non-null error code: " + obj.ToString());
			}
			byte[] array2 = new byte[num];
			Buffer.BlockCopy(array, srcOffset, array2, 0, num);
			string result;
			if (array.Length == 0)
			{
				result = string.Empty;
			}
			else
			{
				result = Encoding.ASCII.GetString(array2, 0, array2.Length);
			}
			return result;
		}

		public bool IsConnected()
		{
			return false;
		}

		public bool IsDisconnected()
		{
			int num = 0;
			while (this.IsConnected() && num < 100)
			{
				Thread.Sleep(100);
				num++;
			}
			return !this.IsConnected();
		}

		public void Close()
		{
			this.connection.Dispose();
		}

		public void Dispose()
		{
			this.Close();
		}

		public void NegotiateSSL()
		{
			IAsyncResult asyncResult = this.connection.BeginNegotiateTlsAsClient(null, null);
			if (!asyncResult.AsyncWaitHandle.WaitOne(50000))
			{
				throw new InvalidOperationException("Negotiate SSL process timed out");
			}
			object obj;
			this.connection.EndNegotiateTlsAsClient(asyncResult, out obj);
			if (obj != null)
			{
				throw new InvalidOperationException("TcpConnection Errorcode was not null");
			}
		}

		public abstract bool LastLineReceived(string responseString, string expectedTag, bool multiLine);

		public abstract TcpResponse CreateResponse(string responseString);

		private byte[] GetRawBytes(int timeout)
		{
			IAsyncResult asyncResult = this.connection.BeginRead(null, null);
			if (!asyncResult.AsyncWaitHandle.WaitOne(timeout * 1000, false))
			{
				throw new ApplicationException(string.Format("No data received in {0} seconds while initializing tcp connection.", timeout));
			}
			byte[] src;
			int srcOffset;
			int num;
			object obj;
			this.connection.EndRead(asyncResult, out src, out srcOffset, out num, out obj);
			if (obj != null)
			{
				throw new ApplicationException("EndRead() resulted in non-null error code: " + obj.ToString());
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(src, srcOffset, array, 0, num);
			return array;
		}

		public const int DefaultTimeout = 120;

		public const int DefaultNegotiateSslTimeout = 50000;

		protected const string StrCrLf = "\r\n";

		private static byte[] byteCrLf = Encoding.ASCII.GetBytes("\r\n");

		private NetworkConnection connection;

		private byte[] data;
	}
}
