using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxyHeaderValue
	{
		public ProxyHeaderValue(ProxyHeaderType proxyHeaderType, byte[] value)
		{
			this.proxyHeaderType = proxyHeaderType;
			this.value = value;
		}

		public byte[] Value
		{
			get
			{
				return this.value;
			}
		}

		public ProxyHeaderType ProxyHeaderType
		{
			get
			{
				return this.proxyHeaderType;
			}
		}

		public void ValidateSize()
		{
			if (this.value != null && this.value.Length > ProxyHeaderValue.MaxSizeLimit)
			{
				throw new InvalidProxySecurityContextException();
			}
		}

		internal static readonly int MaxSizeLimit = 3145728;

		private ProxyHeaderType proxyHeaderType;

		private byte[] value;
	}
}
