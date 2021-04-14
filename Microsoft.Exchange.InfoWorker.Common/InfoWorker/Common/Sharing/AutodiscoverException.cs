using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class AutodiscoverException : SharingSynchronizationException
	{
		public AutodiscoverException(LocalizedString message) : base(message)
		{
		}

		internal AutodiscoverException(LocalizedString message, UserSettings userSettings) : base(message)
		{
			this.Data.Add("User Settings", userSettings);
		}

		private const string UserSettingsAdditionalData = "User Settings";
	}
}
