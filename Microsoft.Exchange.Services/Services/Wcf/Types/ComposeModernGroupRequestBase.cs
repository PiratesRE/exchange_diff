using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class ComposeModernGroupRequestBase : BaseRequest
	{
		[DataMember(Name = "Name", IsRequired = false)]
		public string Name { get; set; }

		[DataMember(Name = "Description", IsRequired = false)]
		public string Description { get; set; }

		[DataMember(Name = "AddedMembers", IsRequired = false)]
		public string[] AddedMembers { get; set; }

		[DataMember(Name = "AddedOwners", IsRequired = false)]
		public string[] AddedOwners { get; set; }
	}
}
