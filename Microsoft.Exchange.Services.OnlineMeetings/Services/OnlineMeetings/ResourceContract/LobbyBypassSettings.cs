using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[CollectionDataContract(Name = "LobbyBypassSettings")]
	internal class LobbyBypassSettings : Resource
	{
		public LobbyBypassSettings(string selfUri) : base(selfUri)
		{
		}
	}
}
