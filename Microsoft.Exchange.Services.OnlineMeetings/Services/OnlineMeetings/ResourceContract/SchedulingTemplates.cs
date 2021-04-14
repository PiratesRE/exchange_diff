using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[CollectionDataContract(Name = "SchedulingTemplates")]
	internal class SchedulingTemplates : Resource
	{
		public SchedulingTemplates(string selfUri) : base(selfUri)
		{
		}
	}
}
