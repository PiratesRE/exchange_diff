using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetPersonaModernGroupMembershipResponse
	{
		[DataMember(Name = "Groups", IsRequired = true)]
		public Persona[] Groups { get; set; }
	}
}
