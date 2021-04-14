using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "CreateUnifiedGroupResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateUnifiedGroupResponse : BaseJsonResponse
	{
		[DataMember(Name = "Persona", IsRequired = false)]
		public Persona Persona { get; set; }

		[DataMember(Name = "Error", IsRequired = false)]
		public string Error { get; set; }

		[DataMember(Name = "ExternalDirectoryObjectId", IsRequired = false)]
		public string ExternalDirectoryObjectId { get; set; }

		[DataMember(Name = "FailureState", IsRequired = false)]
		public CreateUnifiedGroupResponse.GroupProvisionFailureState FailureState { get; set; }

		public enum GroupProvisionFailureState
		{
			NoError,
			FailedCreate,
			FailedMailboxProvision
		}
	}
}
