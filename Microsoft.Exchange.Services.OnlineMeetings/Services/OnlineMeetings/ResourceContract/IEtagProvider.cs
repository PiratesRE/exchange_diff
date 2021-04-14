using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	public interface IEtagProvider
	{
		string ETag { get; set; }
	}
}
