using System;

namespace Microsoft.Exchange.Rpc
{
	internal class RpcHttpQueryString
	{
		public RpcHttpQueryString(string queryString)
		{
			this.RcaServer = string.Empty;
			this.RcaServerPort = string.Empty;
			this.AdditionalParameters = string.Empty;
			if (string.IsNullOrEmpty(queryString))
			{
				return;
			}
			int num = queryString.IndexOf(':', 1);
			if (num == -1)
			{
				num = queryString.Length;
			}
			this.RcaServer = queryString.Substring(1, num - 1);
			int num2 = queryString.IndexOf('&', num);
			if (num2 == -1)
			{
				num2 = queryString.Length;
			}
			int num3 = num2 - (num + 1);
			if (num3 > 0)
			{
				this.RcaServerPort = queryString.Substring(num + 1, num3);
			}
			if (num2 < queryString.Length)
			{
				this.AdditionalParameters = queryString.Substring(num2);
			}
		}

		public string RcaServer { get; private set; }

		public string RcaServerPort { get; private set; }

		public string AdditionalParameters { get; private set; }
	}
}
