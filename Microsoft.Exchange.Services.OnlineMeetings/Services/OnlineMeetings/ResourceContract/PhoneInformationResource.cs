using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "PhoneInformationResource")]
	[Parent("user")]
	[Get(typeof(PhoneInformationResource))]
	internal class PhoneInformationResource : Resource
	{
		public PhoneInformationResource(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "phoneInformation";
	}
}
