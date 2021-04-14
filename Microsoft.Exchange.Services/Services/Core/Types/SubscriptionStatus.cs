using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SubscriptionStatusType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum SubscriptionStatus
	{
		Invalid = -1,
		[XmlEnum("OK")]
		OK,
		[XmlEnum("Unsubscribe")]
		Unsubscribe
	}
}
