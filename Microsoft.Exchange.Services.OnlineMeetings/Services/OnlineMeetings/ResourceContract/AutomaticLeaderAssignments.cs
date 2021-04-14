using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[CollectionDataContract(Name = "AutomaticLeaderAssignments")]
	internal class AutomaticLeaderAssignments : Resource
	{
		public AutomaticLeaderAssignments(string selfUri) : base(selfUri)
		{
		}
	}
}
