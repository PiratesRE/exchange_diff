using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "OnlineMeetingExtensionType")]
	internal enum OnlineMeetingExtensionType
	{
		Undefined,
		RoamedOrganizerData,
		RoamedParticipantData
	}
}
