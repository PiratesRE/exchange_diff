using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Flags]
	[XmlType(TypeName = "NotificationEventTypeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum EventType
	{
		[XmlEnum("CopiedEvent")]
		Copied = 32,
		[XmlEnum("CreatedEvent")]
		Created = 2,
		[XmlEnum("DeletedEvent")]
		Deleted = 4,
		[XmlEnum("ModifiedEvent")]
		Modified = 8,
		[XmlEnum("MovedEvent")]
		Moved = 16,
		[XmlEnum("NewMailEvent")]
		NewMail = 1,
		[XmlEnum("FreeBusyChangedEvent")]
		FreeBusyChanged = 64
	}
}
