using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public enum SortDirection
	{
		[EnumMember]
		Ascending,
		[EnumMember]
		Descending
	}
}
