using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.GetItemEstimate;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.GetItemEstimate
{
	[XmlRoot(ElementName = "GetItemEstimate", Namespace = "GetItemEstimate", IsNullable = false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class GetItemEstimateRequest : GetItemEstimate
	{
		public GetItemEstimateRequest()
		{
			base.Collections = new List<Collection>();
		}
	}
}
