using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal class EmbeddedMultipartRelatedResponse
	{
		public object Root { get; set; }

		public Dictionary<string, object> Parts { get; set; }
	}
}
