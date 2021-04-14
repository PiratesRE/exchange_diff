using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Get(typeof(NormalizePhoneNumber))]
	[Parent("user")]
	[DataContract(Name = "NormalizePhoneNumber")]
	internal class NormalizePhoneNumber : Resource
	{
		public NormalizePhoneNumber(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "NormalizePhoneNumber";
	}
}
