using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "MediaType")]
	internal enum MediaType
	{
		[EnumMember(Value = "Audio")]
		Audio,
		[EnumMember(Value = "MainVideo")]
		MainVideo,
		[EnumMember(Value = "PanoramicVideo")]
		PanoramicVideo,
		[EnumMember(Value = "ApplicationSharing")]
		ApplicationSharing,
		[EnumMember(Value = "Chat")]
		Chat,
		[EnumMember(Value = "WhiteBoarding")]
		WhiteBoarding,
		[EnumMember(Value = "PowerPoint")]
		PowerPoint,
		[EnumMember(Value = "FileSharing")]
		FileSharing,
		[EnumMember(Value = "Polling")]
		Polling
	}
}
