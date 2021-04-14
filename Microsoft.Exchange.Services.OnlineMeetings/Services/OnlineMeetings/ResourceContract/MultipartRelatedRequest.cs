using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "MultipartRelatedRequest")]
	internal class MultipartRelatedRequest<T>
	{
		public T Root { get; set; }

		public Dictionary<string, object> Parts { get; set; }
	}
}
