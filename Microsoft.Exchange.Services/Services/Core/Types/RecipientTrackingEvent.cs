using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RecipientTrackingEventType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class RecipientTrackingEvent
	{
		internal bool TryGetInternalId(out long internalId)
		{
			internalId = 0L;
			return !string.IsNullOrEmpty(this.InternalId) && long.TryParse(this.InternalId, out internalId) && internalId >= 0L;
		}

		public DateTime Date;

		public EmailAddressWrapper Recipient;

		public string DeliveryStatus;

		public string EventDescription;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] EventData;

		public string Server;

		[XmlElement(DataType = "nonNegativeInteger")]
		public string InternalId;

		public string UniquePathId;

		public bool HiddenRecipient;

		public bool BccRecipient;

		public string RootAddress;

		[XmlArrayItem("TrackingPropertyType", IsNullable = false)]
		public TrackingPropertyType[] Properties;
	}
}
