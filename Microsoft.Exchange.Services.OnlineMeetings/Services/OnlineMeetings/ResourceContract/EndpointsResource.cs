using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Get(typeof(EndpointsResource))]
	[Link("webticket")]
	[Link("oauth")]
	[DataContract(Name = "Endpoints")]
	internal class EndpointsResource : Resource
	{
		public EndpointsResource(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "endpoints";
	}
}
