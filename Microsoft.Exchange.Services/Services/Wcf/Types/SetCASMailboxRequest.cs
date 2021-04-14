using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetCASMailboxRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public SetCASMailbox Options { get; set; }
	}
}
