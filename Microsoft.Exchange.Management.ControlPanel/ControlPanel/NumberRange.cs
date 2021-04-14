using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NumberRange
	{
		[DataMember]
		public int AtMost { get; set; }

		[DataMember]
		public int AtLeast { get; set; }
	}
}
