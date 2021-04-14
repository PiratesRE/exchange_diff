using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.DDIService
{
	[DataContract]
	public class MigrationReportGroupDetails
	{
		[DataMember]
		public string Data { get; set; }
	}
}
