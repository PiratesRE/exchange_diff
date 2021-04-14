using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "TrackingPropertyType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class TrackingPropertyType
	{
		public string Name;

		public string Value;
	}
}
