using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsTcpClient : IDisposable
	{
		public ApnsTcpClient(TcpClient tcpClient)
		{
			if (tcpClient == null)
			{
				throw new ArgumentNullException("tcpClient");
			}
			this.TcpClient = tcpClient;
		}

		public virtual bool Connected
		{
			get
			{
				return this.TcpClient.Connected;
			}
		}

		private TcpClient TcpClient { get; set; }

		public virtual IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
		{
			return this.TcpClient.BeginConnect(host, port, requestCallback, state);
		}

		public virtual void EndConnect(IAsyncResult asyncResult)
		{
			this.TcpClient.EndConnect(asyncResult);
			this.TcpClient.SendBufferSize = 320;
			this.TcpClient.ReceiveBufferSize = 128;
			this.TcpClient.Client.IOControl((IOControlCode)((ulong)-1744830460), ApnsTcpClient.KeepAliveValues, null);
		}

		public virtual void Connect(string hostname, int port)
		{
			this.TcpClient.Connect(hostname, port);
		}

		public virtual NetworkStream GetStream()
		{
			return this.TcpClient.GetStream();
		}

		public virtual void Dispose()
		{
			this.TcpClient.Close();
		}

		private static byte[] CreateKeepAliveValues()
		{
			uint num = 0U;
			int num2 = Marshal.SizeOf(num);
			byte[] array = new byte[num2 * 3];
			BitConverter.GetBytes(1U).CopyTo(array, 0);
			BitConverter.GetBytes(240000U).CopyTo(array, num2);
			BitConverter.GetBytes(1000U).CopyTo(array, num2 * 2);
			return array;
		}

		private const int SendBufferSize = 320;

		private const int ReceiveBufferSize = 128;

		private static readonly byte[] KeepAliveValues = ApnsTcpClient.CreateKeepAliveValues();
	}
}
