using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetGroupResponse
	{
		[DataMember]
		public Persona[] Owners { get; set; }

		[DataMember]
		public Persona[] Members { get; set; }

		[DataMember]
		public int MembersCount { get; set; }

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ItemId PersonaId { get; set; }
	}
}
