using System;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class BufferResponseItem : IResponseItem
	{
		public BufferResponseItem() : this(null, 0, 0, null)
		{
		}

		public BufferResponseItem(byte[] buf) : this(buf, 0, buf.Length, null)
		{
		}

		public BufferResponseItem(byte[] buf, BaseSession.SendCompleteDelegate sendCompleteDelegate) : this(buf, 0, buf.Length, sendCompleteDelegate)
		{
		}

		public BufferResponseItem(byte[] buf, int offset, int size) : this(buf, offset, size, null)
		{
		}

		public BufferResponseItem(byte[] buf, int offset, int size, BaseSession.SendCompleteDelegate sendCompleteDelegate)
		{
			this.sendCompleteDelegate = sendCompleteDelegate;
			this.responseBuf = buf;
			this.index = offset;
			this.size = size;
		}

		public BaseSession.SendCompleteDelegate SendCompleteDelegate
		{
			get
			{
				return this.sendCompleteDelegate;
			}
		}

		protected bool DataSent
		{
			get
			{
				return this.dataSent;
			}
		}

		public virtual int GetNextChunk(BaseSession session, out byte[] buffer, out int offset)
		{
			if (this.dataSent)
			{
				buffer = null;
				offset = 0;
				return 0;
			}
			ProtocolSession protocolSession = session as ProtocolSession;
			if (protocolSession != null)
			{
				if (protocolSession.ProxySession != null)
				{
					protocolSession.LogSend("Proxy {0} bytes", this.size);
				}
				else
				{
					protocolSession.LogSend(this.responseBuf, this.index, this.size);
				}
			}
			this.dataSent = true;
			buffer = this.responseBuf;
			offset = this.index;
			return this.size;
		}

		public void ClearData()
		{
			if (this.responseBuf != null)
			{
				Array.Clear(this.responseBuf, this.index, this.size);
			}
		}

		public void BindData(byte[] buf, int offset, int size, BaseSession.SendCompleteDelegate sendCompleteDelegate)
		{
			this.responseBuf = buf;
			this.index = offset;
			this.size = size;
			this.dataSent = false;
			this.sendCompleteDelegate = sendCompleteDelegate;
		}

		private byte[] responseBuf;

		private int size;

		private int index;

		private bool dataSent;

		private BaseSession.SendCompleteDelegate sendCompleteDelegate;
	}
}
