using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "modalityType")]
	internal enum ModalityType
	{
		[EnumMember(Value = "Audio")]
		Audio,
		[EnumMember(Value = "Video")]
		Video,
		[EnumMember(Value = "PhoneAudio")]
		PhoneAudio,
		[EnumMember(Value = "ApplicationSharing")]
		ApplicationSharing,
		[EnumMember(Value = "Messaging")]
		Messaging,
		[EnumMember(Value = "DataCollaboration")]
		DataCollaboration
	}
}
