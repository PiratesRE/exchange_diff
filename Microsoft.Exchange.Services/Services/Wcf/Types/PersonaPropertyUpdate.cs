using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[KnownType(typeof(PersonaPropertyUpdate))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class PersonaPropertyUpdate : PropertyUpdate
	{
		[DataMember(Name = "OldValue", EmitDefaultValue = false, Order = 0)]
		public string OldValue { get; internal set; }

		[DataMember(Name = "NewValue", EmitDefaultValue = false, Order = 1)]
		public string NewValue { get; internal set; }

		[DataMember(Name = "AddMemberToPDL", EmitDefaultValue = false, Order = 2)]
		public EmailAddressWrapper AddMemberToPDL { get; internal set; }

		[DataMember(Name = "DeleteMemberFromPDL", EmitDefaultValue = false, Order = 3)]
		public EmailAddressWrapper DeleteMemberFromPDL { get; internal set; }
	}
}
