using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "EventStatus")]
	internal enum EventStatus
	{
		Failure = -1,
		Undefined,
		Success
	}
}
