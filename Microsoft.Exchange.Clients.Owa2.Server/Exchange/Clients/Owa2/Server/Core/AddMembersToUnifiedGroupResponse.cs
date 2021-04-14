using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Name = "AddMembersToUnifiedGroupResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AddMembersToUnifiedGroupResponse : BaseJsonResponse
	{
		[DataMember(Name = "ErrorState", IsRequired = false)]
		public UnifiedGroupResponseErrorState ErrorState { get; set; }

		[DataMember(Name = "Error", IsRequired = false)]
		public string Error { get; set; }
	}
}
