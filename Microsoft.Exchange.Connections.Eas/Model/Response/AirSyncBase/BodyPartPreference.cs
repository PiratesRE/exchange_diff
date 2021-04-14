using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSyncBase
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSyncBase", TypeName = "BodyPartPreference")]
	public class BodyPartPreference : BodyPreference
	{
	}
}
