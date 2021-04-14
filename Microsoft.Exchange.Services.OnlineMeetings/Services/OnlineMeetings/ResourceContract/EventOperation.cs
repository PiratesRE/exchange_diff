using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "EventOperation")]
	internal enum EventOperation
	{
		Undefined,
		Completed,
		Added,
		Updated,
		Deleted
	}
}
