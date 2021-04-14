using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetNotesForPersonaRequest
	{
		[DataMember]
		public string PersonaId { get; set; }

		[DataMember]
		public int MaxBytesToFetch { get; set; }

		[DataMember]
		public EmailAddressWrapper EmailAddress { get; set; }

		[DataMember(Name = "ParentFolderId", IsRequired = false, EmitDefaultValue = false)]
		public TargetFolderId ParentFolderId { get; set; }
	}
}
