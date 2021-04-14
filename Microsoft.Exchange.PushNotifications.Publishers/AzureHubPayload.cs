using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureHubPayload
	{
		public AzureHubPayload(AzureSasKey[] sasKeys)
		{
			ArgumentValidator.ThrowIfNull("sasKeys", sasKeys);
			ArgumentValidator.ThrowIfOutOfRange<int>("sasKeys.Length", sasKeys.Length, 0, int.MaxValue);
			this.SasKeys = sasKeys;
		}

		public AzureSasKey[] SasKeys { get; private set; }

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = this.SasKeys.ToNullableString(null);
			}
			return this.toString;
		}

		internal void WriteAzureHubPayload(AzureHubPayloadWriter apw)
		{
			ArgumentValidator.ThrowIfNull("apw", apw);
			foreach (AzureSasKey sasKey in this.SasKeys)
			{
				apw.AddAuthorizationRule(sasKey);
			}
		}

		private string toString;
	}
}
