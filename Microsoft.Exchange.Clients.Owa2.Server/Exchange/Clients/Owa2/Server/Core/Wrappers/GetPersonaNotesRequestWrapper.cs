using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetPersonaNotesRequestWrapper
	{
		[DataMember(Name = "personaId")]
		public string PersonaId { get; set; }

		[DataMember(Name = "maxBytesToFetch")]
		public int MaxBytesToFetch { get; set; }
	}
}
