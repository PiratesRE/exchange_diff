using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CollectionDelta
	{
		[DataMember]
		public Identity[] Added { get; set; }

		[DataMember]
		public Identity[] Removed { get; set; }
	}
}
