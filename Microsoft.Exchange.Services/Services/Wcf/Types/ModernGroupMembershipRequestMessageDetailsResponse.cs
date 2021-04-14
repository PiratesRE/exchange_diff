using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Name = "ModernGroupMembershipRequestMessageDetailsResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ModernGroupMembershipRequestMessageDetailsResponse : BaseJsonResponse
	{
		[DataMember(Name = "IsOwner", IsRequired = true)]
		public bool IsOwner { get; set; }

		[DataMember(Name = "GroupPersona", IsRequired = true)]
		public Persona GroupPersona { get; set; }
	}
}
