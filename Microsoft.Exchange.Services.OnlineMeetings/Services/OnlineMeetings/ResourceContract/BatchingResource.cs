using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Post(typeof(BatchingResource))]
	[Parent("Application")]
	internal class BatchingResource : Resource
	{
		public BatchingResource(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "Batch";

		public const bool IsXmlSupported = false;

		public const bool IsJsonSupported = false;

		public const string ContentType = "multipart/batching";
	}
}
