using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[KnownType(typeof(EventsResponse))]
	[DataContract]
	[KnownType(typeof(EmbeddedMultipartRelatedResponse))]
	internal abstract class ResponseContainer
	{
		[IgnoreDataMember]
		public Resource ContainedResource { get; protected set; }

		[DataMember(Name = "Error", EmitDefaultValue = false)]
		public ErrorInformation Error { get; set; }
	}
}
