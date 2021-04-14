using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "Command")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public abstract class Command
	{
	}
}
