using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class AutoDiscoverResult
	{
		public AutoDiscoverResult(AutoDiscoverResultCode resultCode, string resultValue)
		{
			this.ResultCode = resultCode;
			this.ResultValue = resultValue;
		}

		public AutoDiscoverResultCode ResultCode { get; private set; }

		public string ResultValue { get; private set; }

		public Uri GetEwsEndpoint()
		{
			Uri result = null;
			if (this.ResultCode == AutoDiscoverResultCode.Success)
			{
				result = new Uri(this.ResultValue);
			}
			return result;
		}
	}
}
