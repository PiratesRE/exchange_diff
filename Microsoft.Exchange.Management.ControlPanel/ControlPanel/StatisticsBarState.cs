using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public enum StatisticsBarState
	{
		[EnumMember]
		Normal,
		[EnumMember]
		Warning,
		[EnumMember]
		Exceeded
	}
}
