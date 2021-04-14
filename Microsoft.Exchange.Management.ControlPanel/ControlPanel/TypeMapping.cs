using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class TypeMapping
	{
		[DataMember]
		public string Type { get; set; }

		[DataMember]
		public string BaseUrl { get; set; }

		[DataMember]
		public bool? InRole { get; set; }
	}
}
