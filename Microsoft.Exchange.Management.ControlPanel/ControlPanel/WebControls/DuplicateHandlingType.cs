using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[DataContract]
	public enum DuplicateHandlingType
	{
		[EnumMember]
		RemoveDuplicateCaseInsensitive,
		[EnumMember]
		RemoveDuplicateCaseSensitive,
		[EnumMember]
		AllowDuplicate
	}
}
