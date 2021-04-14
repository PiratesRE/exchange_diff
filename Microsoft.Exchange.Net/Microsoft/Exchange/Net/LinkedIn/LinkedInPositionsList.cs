using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[DataContract]
	public class LinkedInPositionsList : IExtensibleDataObject
	{
		[DataMember(Name = "values")]
		public List<LinkedInPosition> Positions { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
