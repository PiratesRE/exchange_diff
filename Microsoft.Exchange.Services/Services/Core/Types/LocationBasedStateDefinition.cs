using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "LocationBasedStateDefinitionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public sealed class LocationBasedStateDefinition : BaseCalendarItemStateDefinition
	{
		[XmlElement]
		[DataMember(IsRequired = true, Order = 1)]
		public string OrganizerLocation { get; set; }

		[DataMember(IsRequired = true, Order = 2)]
		[XmlElement]
		public string AttendeeLocation { get; set; }
	}
}
