using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tracking
{
	[Serializable]
	internal class TrackingLocalizedException : LocalizedException
	{
		internal TrackingLocalizedException(LocalizedString message) : base(message)
		{
		}
	}
}
