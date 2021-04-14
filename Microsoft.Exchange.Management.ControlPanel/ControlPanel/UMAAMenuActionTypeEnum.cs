using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public enum UMAAMenuActionTypeEnum
	{
		[EnumMember]
		None,
		[EnumMember]
		TransferToExtension,
		[EnumMember]
		TransferToAutoAttendant,
		[EnumMember]
		LeaveVoicemailFor,
		[EnumMember]
		AnnounceBusinessLocation,
		[EnumMember]
		AnnounceBusinessHours
	}
}
