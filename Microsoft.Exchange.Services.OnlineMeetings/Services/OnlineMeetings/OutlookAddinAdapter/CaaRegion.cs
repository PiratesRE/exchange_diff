using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[KnownType(typeof(CaaRegion))]
	[XmlType("Region")]
	[DataContract(Name = "Region")]
	public class CaaRegion
	{
		[XmlAttribute("name")]
		[DataMember(Name = "name", EmitDefaultValue = true)]
		public string Name { get; set; }
	}
}
