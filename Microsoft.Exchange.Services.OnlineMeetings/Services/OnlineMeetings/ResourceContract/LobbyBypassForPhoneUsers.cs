using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "lobbyBypassForPhoneUsers")]
	internal enum LobbyBypassForPhoneUsers
	{
		[EnumMember]
		Disabled,
		[EnumMember]
		Enabled
	}
}
