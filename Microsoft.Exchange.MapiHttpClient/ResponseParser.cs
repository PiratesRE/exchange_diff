using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	internal abstract class ResponseParser : BaseObject
	{
		protected ResponseParser(HttpStatusCode httpStatusCode, ResponseCode responseCode, int maxResponseSize)
		{
			this.httpStatusCode = httpStatusCode;
			this.responseCode = responseCode;
			this.maxResponseSize = maxResponseSize;
		}

		public bool IsSuccessful
		{
			get
			{
				base.CheckDisposed();
				return this.httpStatusCode == HttpStatusCode.OK && this.responseCode == ResponseCode.Success;
			}
		}

		public HttpStatusCode HttpStatusCode
		{
			get
			{
				base.CheckDisposed();
				return this.httpStatusCode;
			}
		}

		public ResponseCode ResponseCode
		{
			get
			{
				base.CheckDisposed();
				return this.responseCode;
			}
		}

		public TimeSpan? ElapsedTime
		{
			get
			{
				base.CheckDisposed();
				return this.elapsedTime;
			}
		}

		public Dictionary<string, string> AdditionalHeaders
		{
			get
			{
				base.CheckDisposed();
				return this.additionalHeaders;
			}
		}

		public ArraySegment<byte> ResponseData
		{
			get
			{
				base.CheckDisposed();
				if (this.responseBufferSize > 0)
				{
					return new ArraySegment<byte>(this.responseBuffer.Array, this.responseBuffer.Offset, this.responseBufferSize);
				}
				return Array<byte>.EmptySegment;
			}
		}

		public string FailureInfo
		{
			get
			{
				base.CheckDisposed();
				if (this.failureStream != null)
				{
					byte[] bytes = this.failureStream.ToArray();
					try
					{
						return Encoding.UTF8.GetString(bytes);
					}
					catch (Exception ex)
					{
						return string.Format("[Unable to convert response body to string; exception={0}:{1}]\r\n", ex.GetType().ToString(), ex.Message);
					}
				}
				return string.Empty;
			}
		}

		public abstract void PutData(ArraySegment<byte> dataFragment);

		public abstract void Done();

		public abstract void AppendParserDiagnosticInformation(StringBuilder stringBuilder);

		protected void WriteResponseData(ArraySegment<byte> dataFragment)
		{
			if (!this.IsSuccessful)
			{
				if (this.failureStream == null)
				{
					this.failureStream = new MemoryStream();
				}
				this.failureStream.Write(dataFragment.Array, dataFragment.Offset, dataFragment.Count);
				return;
			}
			if (dataFragment.Count + this.responseBufferSize > this.maxResponseSize)
			{
				throw ProtocolException.FromResponseCode((LID)52768, string.Format("Response data larger than requested; size={0}, expected={1}", dataFragment.Count + this.responseBufferSize, this.maxResponseSize), ResponseCode.InvalidPayload, null);
			}
			if (this.responseBuffer == null)
			{
				int i;
				for (i = 8192; i < dataFragment.Count; i *= 2)
				{
				}
				this.responseBuffer = new WorkBuffer(Math.Min(i, this.maxResponseSize));
			}
			else if (this.responseBuffer.MaxSize - this.responseBufferSize < dataFragment.Count)
			{
				int num = this.responseBuffer.MaxSize;
				while (num - this.responseBufferSize < dataFragment.Count)
				{
					num *= 2;
				}
				WorkBuffer workBuffer = new WorkBuffer(Math.Min(num, this.maxResponseSize));
				Array.Copy(this.responseBuffer.Array, this.responseBuffer.Offset, workBuffer.Array, workBuffer.Offset, this.responseBufferSize);
				Util.DisposeIfPresent(this.responseBuffer);
				this.responseBuffer = workBuffer;
			}
			Array.Copy(dataFragment.Array, dataFragment.Offset, this.responseBuffer.Array, this.responseBuffer.Offset + this.responseBufferSize, dataFragment.Count);
			this.responseBufferSize += dataFragment.Count;
		}

		protected void SetHeader(string header, string headerValue)
		{
			if (this.additionalHeaders == null)
			{
				this.additionalHeaders = new Dictionary<string, string>();
			}
			this.additionalHeaders.Add(header, headerValue);
			if (string.Compare("X-ResponseCode", header, true) != 0)
			{
				if (string.Compare("X-ElapsedTime", header, true) == 0)
				{
					int num;
					if (!int.TryParse(headerValue, out num) || num < 0)
					{
						this.elapsedTime = null;
					}
					this.elapsedTime = new TimeSpan?(TimeSpan.FromMilliseconds((double)num));
				}
				return;
			}
			int num2;
			if (!int.TryParse(headerValue, out num2))
			{
				throw ProtocolException.FromResponseCode((LID)36384, string.Format("Unable to parse a value from additional {1} header; found={0}", headerValue, "X-ResponseCode"), ResponseCode.InvalidHeader, null);
			}
			this.responseCode = (ResponseCode)num2;
		}

		protected override void InternalDispose()
		{
			if (this.failureStream != null)
			{
				this.failureStream.Close();
			}
			Util.DisposeIfPresent(this.responseBuffer);
			base.InternalDispose();
		}

		private readonly int maxResponseSize;

		private readonly HttpStatusCode httpStatusCode;

		private WorkBuffer responseBuffer;

		private int responseBufferSize;

		private MemoryStream failureStream;

		private ResponseCode responseCode;

		private TimeSpan? elapsedTime = null;

		private Dictionary<string, string> additionalHeaders;
	}
}
