using System;

namespace Microsoft.Exchange.Rpc.SharedCache
{
	[Serializable]
	public sealed class CacheResponse
	{
		private CacheResponse(ResponseCode responseCode, byte[] value, string diagnostics)
		{
			this.m_responseCode = responseCode;
			this.m_value = value;
			this.m_diagnostics = diagnostics;
		}

		private CacheResponse()
		{
		}

		public static CacheResponse Create(ResponseCode responseCode, byte[] value)
		{
			return new CacheResponse(responseCode, value, null);
		}

		public static CacheResponse Create(ResponseCode responseCode)
		{
			return new CacheResponse(responseCode, null, null);
		}

		public ResponseCode ResponseCode
		{
			get
			{
				return this.m_responseCode;
			}
		}

		public byte[] Value
		{
			get
			{
				return this.m_value;
			}
		}

		public string Diagnostics
		{
			get
			{
				return this.m_diagnostics;
			}
		}

		public void AppendDiagInfo(string diagKeyName, string diagData)
		{
			string text = string.Format("{0}={1}", diagKeyName, diagData);
			if (string.IsNullOrEmpty(this.m_diagnostics))
			{
				this.m_diagnostics = text;
			}
			else
			{
				this.m_diagnostics += "|" + text;
			}
		}

		public sealed override string ToString()
		{
			string arg;
			if (this.m_diagnostics != null)
			{
				arg = this.m_diagnostics;
			}
			else
			{
				arg = "<null>";
			}
			byte[] value = this.m_value;
			string arg2;
			if (value != null)
			{
				arg2 = value.Length.ToString();
			}
			else
			{
				arg2 = "<null>";
			}
			return string.Format("Code={0}; BlobSize={1} bytes; DiagInfo={2}", ((ResponseCode)this.m_responseCode).ToString(), arg2, arg);
		}

		private ResponseCode m_responseCode;

		private byte[] m_value;

		private string m_diagnostics;
	}
}
