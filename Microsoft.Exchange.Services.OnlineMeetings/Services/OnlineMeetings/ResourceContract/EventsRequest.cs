using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "EventsRequest")]
	[Namespace("Events")]
	internal class EventsRequest : RequestContainer
	{
	}
}
