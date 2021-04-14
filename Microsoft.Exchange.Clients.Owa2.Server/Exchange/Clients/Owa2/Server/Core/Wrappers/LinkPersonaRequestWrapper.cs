using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class LinkPersonaRequestWrapper
	{
		[DataMember(Name = "linkToPersonaId")]
		public ItemId LinkToPersonaId { get; set; }

		[DataMember(Name = "personaIdToBeLinked")]
		public ItemId PersonaIdToBeLinked { get; set; }
	}
}
