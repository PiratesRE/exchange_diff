using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Transport.Agent.Hygiene;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.Background
{
	internal class TcpConnection : TransportConnection, IDisposable
	{
		public override void AsyncConnect(IPEndPoint remoteEndpoint, TcpConnection tcpCxn, NetworkCredential authInfo)
		{
			try
			{
				this.socket.BeginConnect(remoteEndpoint, new AsyncCallback(this.AsyncConnectCallback), this);
			}
			catch (SocketException)
			{
				this.Shutdown(true);
			}
		}

		public TcpConnection(ProxyChain proxyChain) : base(proxyChain)
		{
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Interlocked.Increment(ref ProtocolAnalysisBgAgent.NumSockets);
			this.m_fReadPending = false;
			this.m_fWritePending = false;
			this.m_fClosing = false;
			this.m_fNotifyNeeded = false;
			this.m_fRemoteReachable = false;
			this.writeBuffer = new byte[512];
			this.writeBufferWritePos = 0;
			this.writeBufferReadPos = 0;
			this.readBuffer = new byte[512];
			this.readBufferWritePos = 0;
		}

		public void Dispose()
		{
			if (this.socket != null)
			{
				this.Shutdown(false);
			}
		}

		public override string ToString()
		{
			if (this.socket != null && this.socket.Connected)
			{
				return this.socket.RemoteEndPoint.ToString();
			}
			return "not connected";
		}

		public bool RemoteReachable
		{
			get
			{
				return this.m_fRemoteReachable;
			}
		}

		protected void ExpandReadBuffer()
		{
			if (this.readBuffer.Length < 1024)
			{
				byte[] array = new byte[this.readBuffer.Length * 2];
				this.readBuffer.CopyTo(array, 0);
				this.readBuffer = array;
			}
		}

		public void Shutdown(bool isNotify)
		{
			bool flag = false;
			lock (this.syncObject)
			{
				if (!this.m_fClosing)
				{
					this.m_fClosing = true;
					this.m_fNotifyNeeded = isNotify;
				}
				if (this.socket != null)
				{
					try
					{
						this.socket.Shutdown(SocketShutdown.Both);
					}
					catch (SocketException)
					{
					}
					if (!this.m_fReadPending && !this.m_fWritePending)
					{
						this.socket.Close();
						this.socket = null;
						Interlocked.Decrement(ref ProtocolAnalysisBgAgent.NumSockets);
						flag = true;
					}
				}
			}
			if (flag && this.m_fNotifyNeeded)
			{
				base.ProxyChain.OnDisconnected();
			}
		}

		private void ExpandWriteBuffer(int len)
		{
			int num = Math.Max(len, 512);
			byte[] array = new byte[this.writeBuffer.Length + num];
			this.writeBuffer.CopyTo(array, 0);
			this.writeBuffer = array;
		}

		private void AsyncConnectCallback(IAsyncResult ar)
		{
			this.m_fRemoteReachable = true;
			if (this.m_fClosing)
			{
				return;
			}
			try
			{
				this.socket.EndConnect(ar);
			}
			catch (SocketException)
			{
				this.Shutdown(true);
				return;
			}
			this.AsyncRead();
			base.ProxyChain.OnConnected(null, 0, 0);
		}

		public void SendMessage(byte[] data, int start, int len)
		{
			lock (this.syncObject)
			{
				if (this.m_fClosing)
				{
					throw new AtsException(AgentStrings.WritingDisallowedOnClosedConnection);
				}
				if (this.writeBufferWritePos + len > this.writeBuffer.Length)
				{
					this.ExpandWriteBuffer(len);
				}
				Array.Copy(data, start, this.writeBuffer, this.writeBufferWritePos, len);
				this.writeBufferWritePos += len;
				if (!this.m_fWritePending)
				{
					this.AsyncWrite(this.writeBuffer, this.writeBufferReadPos, this.writeBufferWritePos - this.writeBufferReadPos);
				}
			}
		}

		public void SendString(string data)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(data);
			this.SendMessage(bytes, 0, bytes.Length);
		}

		public void SendByte(byte data)
		{
			this.SendMessage(new byte[]
			{
				data
			}, 0, 1);
		}

		private void AsyncWrite(byte[] data, int start, int len)
		{
			if (this.m_fClosing)
			{
				return;
			}
			this.m_fWritePending = true;
			try
			{
				this.socket.BeginSend(data, start, len, SocketFlags.None, new AsyncCallback(this.AsyncWriteCallback), this);
			}
			catch (SocketException)
			{
				this.m_fWritePending = false;
				this.Shutdown(true);
			}
		}

		private void AsyncWriteCallback(IAsyncResult ar)
		{
			lock (this.syncObject)
			{
				int num = 0;
				try
				{
					this.m_fWritePending = false;
					num = this.socket.EndSend(ar);
				}
				catch (SocketException)
				{
					this.Shutdown(true);
					return;
				}
				this.writeBufferReadPos += num;
				if (this.m_fClosing)
				{
					this.Shutdown(true);
				}
				else
				{
					Array.Copy(this.writeBuffer, this.writeBufferReadPos, this.writeBuffer, 0, this.writeBufferWritePos - this.writeBufferReadPos);
					this.writeBufferWritePos -= this.writeBufferReadPos;
					this.writeBufferReadPos = 0;
					if (this.writeBufferReadPos < this.writeBufferWritePos)
					{
						this.AsyncWrite(this.writeBuffer, this.writeBufferReadPos, this.writeBufferWritePos - this.writeBufferReadPos);
					}
				}
			}
		}

		private void AsyncRead()
		{
			if (this.m_fClosing)
			{
				return;
			}
			int num = this.readBuffer.Length - this.readBufferWritePos;
			if (num < 1)
			{
				this.ExpandReadBuffer();
			}
			try
			{
				this.m_fReadPending = true;
				this.socket.BeginReceive(this.readBuffer, this.readBufferWritePos, this.readBuffer.Length - this.readBufferWritePos - 1, SocketFlags.None, new AsyncCallback(this.AsyncReadCallback), this);
			}
			catch (SocketException)
			{
				this.m_fReadPending = false;
				this.Shutdown(true);
			}
		}

		private void AsyncReadCallback(IAsyncResult ar)
		{
			int num;
			lock (this.syncObject)
			{
				try
				{
					this.m_fReadPending = false;
					num = this.socket.EndReceive(ar);
					if (num == 0)
					{
						this.Shutdown(true);
						return;
					}
				}
				catch (SocketException)
				{
					this.Shutdown(true);
					return;
				}
			}
			this.readBufferWritePos += num;
			this.OnDataAvailable();
			if (this.m_fClosing)
			{
				this.Shutdown(true);
				return;
			}
			this.AsyncRead();
		}

		private void OnDataAvailable()
		{
			int num = 0;
			if (base.DataCxn != null && this.readBufferWritePos > num)
			{
				int num2 = base.DataCxn.OnDataReceived(this.readBuffer, num, this.readBufferWritePos - num);
				num += num2;
				Array.Copy(this.readBuffer, num, this.readBuffer, 0, this.readBufferWritePos - num);
				this.readBufferWritePos -= num;
			}
		}

		private const int MinExpandSize = 512;

		private const int MaxBufferSize = 1024;

		private Socket socket;

		private byte[] readBuffer;

		private int readBufferWritePos;

		private byte[] writeBuffer;

		private int writeBufferWritePos;

		private int writeBufferReadPos;

		private bool m_fWritePending;

		private bool m_fReadPending;

		private bool m_fClosing;

		private bool m_fNotifyNeeded;

		private bool m_fRemoteReachable;

		private object syncObject = new object();
	}
}
