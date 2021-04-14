using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum OpenTransportSessionFlags
	{
		None,
		OpenForQuotaMessageDelivery,
		OpenForNormalMessageDelivery,
		OpenForSpecialMessageDelivery
	}
}
