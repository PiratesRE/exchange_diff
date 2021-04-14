using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ArrayOfTrackingPropertiesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class ArrayOfTrackingPropertiesType
	{
		[XmlElement("TrackingPropertyType", IsNullable = false)]
		public TrackingPropertyType[] Items;
	}
}
