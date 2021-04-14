using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AggregatedScope
	{
		[DataMember]
		public bool IsOrganizationalUnit;

		[DataMember]
		public string ID;
	}
}
