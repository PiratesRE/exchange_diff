using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.Settings
{
	[XmlType(Namespace = "Settings", TypeName = "Settings")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Settings
	{
		[XmlElement(ElementName = "UserInformation")]
		public UserInformation UserInformation { get; set; }
	}
}
