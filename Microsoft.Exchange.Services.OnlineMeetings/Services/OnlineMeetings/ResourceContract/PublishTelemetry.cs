using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("application")]
	[Post(typeof(Telemetry))]
	internal class PublishTelemetry : Resource
	{
		public PublishTelemetry(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "publishTelemetry";
	}
}
