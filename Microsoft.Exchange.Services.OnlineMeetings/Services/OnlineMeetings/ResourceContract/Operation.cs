using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract]
	internal abstract class Operation : Resource
	{
		protected Operation(string selfUri) : base(selfUri)
		{
		}
	}
}
