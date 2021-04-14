using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface IOriginatingChangeTimestamp
	{
		DateTime? LastExchangeChangedTime { get; set; }
	}
}
