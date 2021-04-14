using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol
{
	[DataContract]
	public class DarTaskAggregateParams : DarTaskParamsBase
	{
		[DataMember]
		public bool IsEnabled { get; set; }
	}
}
