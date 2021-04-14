using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSync;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Sync
{
	[XmlRoot(ElementName = "Sync", Namespace = "AirSync", IsNullable = false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class SyncRequest : Sync
	{
		public SyncRequest()
		{
			base.Collections = new List<Collection>();
		}
	}
}
