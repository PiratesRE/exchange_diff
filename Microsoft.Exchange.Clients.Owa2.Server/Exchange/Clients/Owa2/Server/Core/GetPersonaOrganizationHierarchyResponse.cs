using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetPersonaOrganizationHierarchyResponse
	{
		[DataMember]
		public Persona[] ManagementChain { get; set; }

		[DataMember]
		public Persona Manager { get; set; }

		[DataMember]
		public Persona[] Peers { get; set; }

		[DataMember]
		public Persona[] DirectReports { get; set; }
	}
}
