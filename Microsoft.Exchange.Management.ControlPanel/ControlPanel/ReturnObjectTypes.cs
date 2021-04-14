using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public enum ReturnObjectTypes
	{
		[EnumMember]
		Full,
		[EnumMember]
		PartialForList,
		[EnumMember]
		None
	}
}
