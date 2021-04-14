using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[CollectionDataContract(Name = "ConferenceAccessLevels")]
	internal class ConferenceAccessLevels : Resource
	{
		public ConferenceAccessLevels(string selfUri) : base(selfUri)
		{
		}
	}
}
