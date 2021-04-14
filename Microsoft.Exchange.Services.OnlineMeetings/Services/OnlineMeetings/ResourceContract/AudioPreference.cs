using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "audioPreference")]
	internal enum AudioPreference
	{
		[EnumMember(Value = "PhoneAudio")]
		PhoneAudio,
		[EnumMember(Value = "VoipAudio")]
		VoipAudio
	}
}
