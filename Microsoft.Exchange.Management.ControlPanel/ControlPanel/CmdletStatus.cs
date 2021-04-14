using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public enum CmdletStatus
	{
		[EnumMember]
		Failed,
		[EnumMember]
		Stopped,
		[EnumMember]
		Completed
	}
}
