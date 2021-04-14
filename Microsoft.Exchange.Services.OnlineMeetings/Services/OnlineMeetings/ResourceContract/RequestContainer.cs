using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract]
	[KnownType(typeof(EventsRequest))]
	internal abstract class RequestContainer
	{
	}
}
