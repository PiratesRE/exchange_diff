using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DistinguishedPropertySetType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum DistinguishedPropertySet
	{
		Meeting,
		Appointment,
		Common,
		PublicStrings,
		Address,
		InternetHeaders,
		CalendarAssistant,
		UnifiedMessaging,
		Task,
		Sharing
	}
}
