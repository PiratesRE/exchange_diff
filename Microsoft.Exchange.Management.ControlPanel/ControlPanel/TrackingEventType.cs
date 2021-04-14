using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public enum TrackingEventType
	{
		[EnumMember]
		SmtpReceive,
		[EnumMember]
		SmtpSend,
		[EnumMember]
		Fail,
		[EnumMember]
		Deliver,
		[EnumMember]
		Resolve,
		[EnumMember]
		Expand,
		[EnumMember]
		Redirect,
		[EnumMember]
		Submit,
		[EnumMember]
		Defer,
		[EnumMember]
		InitMessageCreated,
		[EnumMember]
		ModeratorRejected,
		[EnumMember]
		ModeratorApprove,
		[EnumMember]
		Pending,
		[EnumMember]
		Transferred,
		[EnumMember]
		None = 99
	}
}
