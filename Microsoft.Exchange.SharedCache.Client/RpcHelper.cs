using System;
using Microsoft.Exchange.Rpc.SharedCache;

namespace Microsoft.Exchange.SharedCache.Client
{
	internal static class RpcHelper
	{
		public static bool ValidateValuedBasedResponse(CacheResponse response)
		{
			if (response == null)
			{
				return false;
			}
			if (response.ResponseCode == ResponseCode.OK)
			{
				byte[] value = response.Value;
			}
			return response.ResponseCode == ResponseCode.OK && response.Value != null;
		}

		public static string CreateTransactionString(string clientName, string action, Guid requestCorrelationId, string key)
		{
			return string.Concat(new string[]
			{
				clientName,
				"_ReqId(",
				requestCorrelationId.ToString(),
				")_",
				action,
				"(",
				key,
				")"
			});
		}

		public static void SetCommonOutParameters(CacheResponse response, out byte[] valueBlob, out string diagnosticsString)
		{
			if (response != null)
			{
				valueBlob = response.Value;
				diagnosticsString = (response.Diagnostics ?? string.Empty);
				return;
			}
			valueBlob = null;
			diagnosticsString = string.Empty;
		}
	}
}
